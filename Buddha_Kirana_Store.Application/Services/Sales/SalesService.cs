using Buddha_Kirana_Store.Abstractions;
using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

internal sealed class SalesService(
    IUnitOfWorkFactory uowFactory,
    IStockService stock,
    IDocumentNumberGenerator numbers,
    ICashBookRecorder cashBook,
    IEnumerable<IBillingStrategy> billingStrategies) : ISalesService
{
    public async Task<SalesInvoiceSearchResult> SearchSalesInvoicesAsync(SalesInvoiceQuery query, int page, int pageSize)
    {
        (page, pageSize) = PagingRules.Normalize(page, pageSize);

        await using var uow = uowFactory.Create();
        var summary = await uow.SalesInvoices.SummarizeAsync(query);
        var invoices = await uow.SalesInvoices.SearchAsync(query, (page - 1) * pageSize, pageSize);

        return new SalesInvoiceSearchResult(
            new PagedResult<SalesInvoice>(invoices, summary.InvoiceCount, page, pageSize),
            summary);
    }

    public async Task<int> SaveSalesInvoiceAsync(SalesInvoice sale, PaymentMode paymentMode)
    {
        if (sale.Items.Count == 0)
            throw new DomainException("Please add at least one item to the sale.");

        var billing = billingStrategies.FirstOrDefault(s => s.Mode == sale.BillingMode)
                      ?? throw new DomainException($"Billing mode '{sale.BillingMode}' is not supported.");

        await using var uow = uowFactory.Create();
        await using var tx = await uow.BeginTransactionAsync();

        var customer = await uow.GetCustomerAsync(sale.CustomerId);
        var items = await uow.LoadItemsAsync(sale.Items.Select(l => l.ItemId));

        // Freeze the cost at the moment of sale so profit stays correct if average cost changes later.
        decimal totalCost = 0;
        foreach (var line in sale.Items)
        {
            var item = items[line.ItemId];
            item.EnsureValidQuantity(line.Quantity);
            line.CostPriceAtSale = item.AvgCostPrice;
            totalCost += line.Quantity * item.AvgCostPrice;
        }
        sale.TotalCostAmount = Math.Round(totalCost, 2);

        billing.Apply(sale, items);

        if (sale.NetTotalAmount <= 0)
            throw new DomainException("Total Invoice Amount must be greater than 0.");

        sale.ApplyPayment();
        customer.AddCredit(sale.CreditAmount); // walk-in customers are rejected here

        sale.InvoiceNumber = await numbers.NextAsync(uow, DocumentType.SalesInvoice);
        uow.SalesInvoices.Add(sale);
        await uow.SaveChangesAsync(); // gives the invoice its id for the ledger reference

        foreach (var line in sale.Items)
            uow.StockLedgers.Add(stock.Decrease(items[line.ItemId], line.Quantity, StockTransactionType.Sale, sale.Id));

        if (sale.ReceivedAmount > 0)
        {
            uow.Payments.Add(new PaymentTransaction
            {
                PartyType = PartyType.Customer,
                PartyId = sale.CustomerId,
                ReferenceInvoiceId = sale.Id,
                Amount = sale.ReceivedAmount,
                PaymentMode = paymentMode,
                Remarks = $"Sale Invoice {sale.InvoiceNumber}"
            });
        }

        cashBook.Record(uow, CashFlowDirection.In, sale.InvoiceDate, paymentMode, sale.ReceivedAmount,
            CashBookSource.SalesReceipt, sale.Id, $"Sale {sale.InvoiceNumber}");

        await uow.SaveChangesAsync();
        await tx.CommitAsync();
        return sale.Id;
    }
}

internal sealed class SalesReturnService(
    IUnitOfWorkFactory uowFactory,
    IStockService stock,
    IDocumentNumberGenerator numbers,
    ICashBookRecorder cashBook) : ISalesReturnService
{
    public async Task<List<SalesReturn>> GetSalesReturnsAsync()
    {
        await using var uow = uowFactory.Create();
        return await uow.SalesReturns.GetAllWithDetailsAsync();
    }

    public async Task<int> SaveSalesReturnAsync(SalesReturn salesReturn)
    {
        salesReturn.CalculateTotals();

        await using var uow = uowFactory.Create();
        await using var tx = await uow.BeginTransactionAsync();

        var customer = await uow.GetCustomerAsync(salesReturn.CustomerId);
        var items = await uow.LoadItemsAsync(salesReturn.Items.Select(l => l.ItemId));

        customer.AdjustAgainstReturn(salesReturn.UdhaarAdjustedAmount); // walk-in customers are rejected here

        salesReturn.ReturnNumber = await numbers.NextAsync(uow, DocumentType.SalesReturn);
        uow.SalesReturns.Add(salesReturn);
        await uow.SaveChangesAsync();

        foreach (var line in salesReturn.Items)
        {
            uow.StockLedgers.Add(stock.Increase(
                items[line.ItemId], line.Quantity, StockTransactionType.SalesReturn,
                salesReturn.Id, salesReturn.ReturnDate));
        }

        cashBook.Record(uow, CashFlowDirection.Out, salesReturn.ReturnDate, salesReturn.RefundMode, salesReturn.RefundPaidAmount,
            CashBookSource.SalesReturnRefund, salesReturn.Id, $"Refund for sales return {salesReturn.ReturnNumber}");

        await uow.SaveChangesAsync();
        await tx.CommitAsync();
        return salesReturn.Id;
    }
}

using Buddha_Kirana_Store.Abstractions;
using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

internal sealed class PurchaseService(
    IUnitOfWorkFactory uowFactory,
    IStockService stock,
    ICashBookRecorder cashBook) : IPurchaseService
{
    public async Task<PurchaseInvoiceSearchResult> SearchPurchaseInvoicesAsync(PurchaseInvoiceQuery query, int page, int pageSize)
    {
        (page, pageSize) = PagingRules.Normalize(page, pageSize);

        await using var uow = uowFactory.Create();
        var summary = await uow.PurchaseInvoices.SummarizeAsync(query);
        var invoices = await uow.PurchaseInvoices.SearchAsync(query, (page - 1) * pageSize, pageSize);

        return new PurchaseInvoiceSearchResult(
            new PagedResult<PurchaseInvoice>(invoices, summary.InvoiceCount, page, pageSize),
            summary);
    }

    public async Task<int> SavePurchaseInvoiceAsync(
        PurchaseInvoice invoice,
        IReadOnlyDictionary<int, decimal> updatedSellingPrices,
        PaymentMode paymentMode)
    {
        invoice.CalculateTotals();

        await using var uow = uowFactory.Create();
        await using var tx = await uow.BeginTransactionAsync();

        var supplier = await uow.GetSupplierAsync(invoice.SupplierId);
        var items = await uow.LoadItemsAsync(invoice.Items.Select(l => l.ItemId));

        uow.PurchaseInvoices.Add(invoice);
        await uow.SaveChangesAsync(); // gives the invoice its id for the ledger reference

        foreach (var line in invoice.Items)
        {
            var item = items[line.ItemId];

            uow.StockLedgers.Add(stock.Receive(item, line.Quantity, line.PurchaseUnitPrice, StockTransactionType.Purchase, invoice.Id));

            if (updatedSellingPrices.TryGetValue(item.Id, out var newSellingPrice) && newSellingPrice > 0)
                item.SellingPrice = newSellingPrice;
        }

        supplier.AddPayable(invoice.BalanceDue);

        if (invoice.PaidAmount > 0)
        {
            uow.Payments.Add(new PaymentTransaction
            {
                PartyType = PartyType.Supplier,
                PartyId = invoice.SupplierId,
                ReferenceInvoiceId = invoice.Id,
                Amount = invoice.PaidAmount,
                PaymentMode = paymentMode,
                Remarks = $"Purchase Bill #{invoice.SupplierInvoiceNo ?? invoice.Id.ToString()}"
            });
        }

        cashBook.Record(uow, CashFlowDirection.Out, invoice.InvoiceDate, paymentMode, invoice.PaidAmount,
            CashBookSource.PurchasePayment, invoice.Id, $"Purchase bill {invoice.SupplierInvoiceNo ?? invoice.Id.ToString()}");

        await uow.SaveChangesAsync();
        await tx.CommitAsync();
        return invoice.Id;
    }
}

internal sealed class PurchaseReturnService(
    IUnitOfWorkFactory uowFactory,
    IStockService stock,
    IDocumentNumberGenerator numbers,
    ICashBookRecorder cashBook) : IPurchaseReturnService
{
    public async Task<List<PurchaseReturn>> GetPurchaseReturnsAsync()
    {
        await using var uow = uowFactory.Create();
        return await uow.PurchaseReturns.GetAllWithDetailsAsync();
    }

    public async Task<int> SavePurchaseReturnAsync(PurchaseReturn purchaseReturn)
    {
        purchaseReturn.CalculateTotals();

        await using var uow = uowFactory.Create();
        await using var tx = await uow.BeginTransactionAsync();

        var supplier = await uow.GetSupplierAsync(purchaseReturn.SupplierId);
        var items = await uow.LoadItemsAsync(purchaseReturn.Items.Select(l => l.ItemId));

        purchaseReturn.ReturnNumber = await numbers.NextAsync(uow, DocumentType.PurchaseReturn);
        uow.PurchaseReturns.Add(purchaseReturn);
        await uow.SaveChangesAsync();

        foreach (var line in purchaseReturn.Items)
        {
            uow.StockLedgers.Add(stock.Decrease(
                items[line.ItemId], line.Quantity, StockTransactionType.PurchaseReturn,
                purchaseReturn.Id, purchaseReturn.ReturnDate));
        }

        supplier.AdjustAgainstReturn(purchaseReturn.PayableAdjustedAmount);

        cashBook.Record(uow, CashFlowDirection.In, purchaseReturn.ReturnDate, purchaseReturn.RefundMode, purchaseReturn.RefundReceivedAmount,
            CashBookSource.PurchaseReturnRefund, purchaseReturn.Id, $"Refund from supplier for return {purchaseReturn.ReturnNumber}");

        await uow.SaveChangesAsync();
        await tx.CommitAsync();
        return purchaseReturn.Id;
    }
}

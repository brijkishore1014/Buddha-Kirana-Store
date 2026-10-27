using Buddha_Kirana_Store.Abstractions;
using Buddha_Kirana_Store.Domain;
using Buddha_Kirana_Store.Services;
using Microsoft.EntityFrameworkCore;

namespace Buddha_Kirana_Store.Data.Repositories;

internal sealed class PurchaseInvoiceRepository(KiranaDbContext db) : EfRepository<PurchaseInvoice>(db), IPurchaseInvoiceRepository
{
    public Task<List<PurchaseInvoice>> SearchAsync(PurchaseInvoiceQuery query, int skip, int take) =>
        ApplyFilter(Db.PurchaseInvoices.AsNoTracking(), query)
            .Include(p => p.Supplier)
            .Include(p => p.Items).ThenInclude(i => i.Item).ThenInclude(it => it.Unit)
            .OrderByDescending(p => p.InvoiceDate)
            .ThenByDescending(p => p.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

    public async Task<PurchaseSummary> SummarizeAsync(PurchaseInvoiceQuery query)
    {
        var filtered = ApplyFilter(Db.PurchaseInvoices.AsNoTracking(), query);

        var count = await filtered.CountAsync();
        var totalBill = await filtered.SumAsync(p => p.TotalAmount);
        var totalPaid = await filtered.SumAsync(p => p.PaidAmount);
        var totalDue = await filtered.SumAsync(p => p.BalanceDue);

        return new PurchaseSummary(count, totalBill, totalPaid, totalDue);
    }

    private static IQueryable<PurchaseInvoice> ApplyFilter(IQueryable<PurchaseInvoice> source, PurchaseInvoiceQuery filter)
    {
        var result = source;

        if (filter.From is { } fromValue)
        {
            var fromDate = fromValue.Date;
            result = result.Where(p => p.InvoiceDate >= fromDate);
        }

        if (filter.To is { } toValue)
        {
            var toExclusive = toValue.Date.AddDays(1);
            result = result.Where(p => p.InvoiceDate < toExclusive);
        }

        if (filter.SupplierId is { } supplierId)
            result = result.Where(p => p.SupplierId == supplierId);

        if (filter.Status is { } status)
            result = result.Where(p => p.PaymentStatus == status);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var pattern = LikePattern.Contains(filter.Search);
            result = result.Where(p =>
                (p.SupplierInvoiceNo != null && EF.Functions.ILike(p.SupplierInvoiceNo, pattern)) ||
                EF.Functions.ILike(p.Supplier.Name, pattern) ||
                p.Items.Any(i => EF.Functions.ILike(i.Item.Name, pattern)));
        }

        return result;
    }
}

internal sealed class SalesInvoiceRepository(KiranaDbContext db) : EfRepository<SalesInvoice>(db), ISalesInvoiceRepository
{
    public Task<List<SalesInvoice>> SearchAsync(SalesInvoiceQuery query, int skip, int take) =>
        ApplyFilter(Db.SalesInvoices.AsNoTracking(), query)
            .Include(s => s.Customer)
            .Include(s => s.Items).ThenInclude(i => i.Item).ThenInclude(it => it.Unit)
            .OrderByDescending(s => s.InvoiceDate)
            .ThenByDescending(s => s.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

    public async Task<SalesSummary> SummarizeAsync(SalesInvoiceQuery query)
    {
        var filtered = ApplyFilter(Db.SalesInvoices.AsNoTracking(), query);

        var count = await filtered.CountAsync();
        var totalNet = await filtered.SumAsync(s => s.NetTotalAmount);
        var totalCredit = await filtered.SumAsync(s => s.CreditAmount);
        var totalCost = await filtered.SumAsync(s => s.TotalCostAmount);

        return new SalesSummary(count, totalNet, totalCredit, totalNet - totalCost);
    }

    private static IQueryable<SalesInvoice> ApplyFilter(IQueryable<SalesInvoice> source, SalesInvoiceQuery filter)
    {
        var result = source;

        if (filter.From is { } fromValue)
        {
            var fromDate = fromValue.Date;
            result = result.Where(s => s.InvoiceDate >= fromDate);
        }

        if (filter.To is { } toValue)
        {
            var toExclusive = toValue.Date.AddDays(1);
            result = result.Where(s => s.InvoiceDate < toExclusive);
        }

        if (filter.CustomerId is { } customerId)
            result = result.Where(s => s.CustomerId == customerId);

        if (filter.Mode is { } mode)
            result = result.Where(s => s.BillingMode == mode);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var pattern = LikePattern.Contains(filter.Search);
            result = result.Where(s =>
                EF.Functions.ILike(s.InvoiceNumber, pattern) ||
                EF.Functions.ILike(s.Customer.Name, pattern) ||
                s.Items.Any(i => EF.Functions.ILike(i.Item.Name, pattern)));
        }

        return result;
    }
}

internal sealed class SalesReturnRepository(KiranaDbContext db) : EfRepository<SalesReturn>(db), ISalesReturnRepository
{
    public Task<List<SalesReturn>> GetAllWithDetailsAsync() =>
        Db.SalesReturns
          .Include(r => r.Customer)
          .Include(r => r.Items).ThenInclude(i => i.Item).ThenInclude(it => it.Unit)
          .OrderByDescending(r => r.ReturnDate)
          .ThenByDescending(r => r.Id)
          .AsNoTracking()
          .ToListAsync();
}

internal sealed class PurchaseReturnRepository(KiranaDbContext db) : EfRepository<PurchaseReturn>(db), IPurchaseReturnRepository
{
    public Task<List<PurchaseReturn>> GetAllWithDetailsAsync() =>
        Db.PurchaseReturns
          .Include(r => r.Supplier)
          .Include(r => r.Items).ThenInclude(i => i.Item).ThenInclude(it => it.Unit)
          .OrderByDescending(r => r.ReturnDate)
          .ThenByDescending(r => r.Id)
          .AsNoTracking()
          .ToListAsync();
}

internal sealed class PaymentTransactionRepository(KiranaDbContext db) : EfRepository<PaymentTransaction>(db), IPaymentTransactionRepository
{
    public Task<List<PaymentTransaction>> GetByPartyTypeAsync(PartyType partyType) =>
        Db.PaymentTransactions
          .Where(p => p.PartyType == partyType)
          .OrderByDescending(p => p.TransactionDate)
          .ThenByDescending(p => p.Id)
          .AsNoTracking()
          .ToListAsync();
}

internal sealed class ExpenseRepository(KiranaDbContext db) : EfRepository<ShopExpense>(db), IExpenseRepository
{
    public Task<List<ShopExpense>> GetAllAsync() =>
        Db.ShopExpenses
          .OrderByDescending(e => e.ExpenseDate)
          .ThenByDescending(e => e.Id)
          .AsNoTracking()
          .ToListAsync();
}

internal sealed class DocumentSequenceRepository(KiranaDbContext db) : EfRepository<DocumentSequence>(db), IDocumentSequenceRepository
{
    public Task<DocumentSequence?> GetByKeyAsync(string sequenceKey) =>
        Db.DocumentSequences.FirstOrDefaultAsync(s => s.SequenceKey == sequenceKey);
}

internal sealed class StockLedgerRepository(KiranaDbContext db) : EfRepository<StockLedger>(db);

internal sealed class CashBookRepository(KiranaDbContext db) : EfRepository<CashBookEntry>(db), ICashBookRepository
{
    public Task<List<CashBookEntry>> SearchAsync(CashBookQuery query, int skip, int take) =>
        ApplyFilter(Db.CashBookEntries.AsNoTracking(), query)
            .OrderBy(e => e.EntryDate)
            .ThenBy(e => e.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

    public Task<int> CountAsync(CashBookQuery query) =>
        ApplyFilter(Db.CashBookEntries.AsNoTracking(), query).CountAsync();

    public Task<List<CashBookModeTotal>> GetModeTotalsAsync(DateTime? fromInclusive, DateTime? toExclusive, PaymentMode? mode)
    {
        var entries = Db.CashBookEntries.AsNoTracking();

        if (fromInclusive is { } fromValue)
            entries = entries.Where(e => e.EntryDate >= fromValue);

        if (toExclusive is { } toValue)
            entries = entries.Where(e => e.EntryDate < toValue);

        if (mode is { } onlyMode)
            entries = entries.Where(e => e.PaymentMode == onlyMode);

        return entries
            .GroupBy(e => e.PaymentMode)
            .Select(g => new CashBookModeTotal(
                g.Key,
                g.Sum(e => e.Direction == CashFlowDirection.In ? e.Amount : 0m),
                g.Sum(e => e.Direction == CashFlowDirection.Out ? e.Amount : 0m)))
            .ToListAsync();
    }

    public Task<List<CashBookEntry>> GetBySourceAsync(CashBookSource source, int sourceId) =>
        Db.CashBookEntries.Where(e => e.Source == source && e.SourceId == sourceId).ToListAsync();

    private static IQueryable<CashBookEntry> ApplyFilter(IQueryable<CashBookEntry> source, CashBookQuery filter)
    {
        var result = source;

        if (filter.From is { } fromValue)
        {
            var fromDate = fromValue.Date;
            result = result.Where(e => e.EntryDate >= fromDate);
        }

        if (filter.To is { } toValue)
        {
            var toExclusive = toValue.Date.AddDays(1);
            result = result.Where(e => e.EntryDate < toExclusive);
        }

        if (filter.Mode is { } mode)
            result = result.Where(e => e.PaymentMode == mode);

        return result;
    }
}

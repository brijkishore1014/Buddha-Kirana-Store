using Buddha_Kirana_Store.Domain;
using Buddha_Kirana_Store.Services;

namespace Buddha_Kirana_Store.Abstractions;

// Each repository only exposes the queries its aggregate really needs (Interface Segregation).
// Application code never sees EF Core types.

public interface IUnitOfMeasureRepository : IRepository<UnitOfMeasure>
{
    Task<List<UnitOfMeasure>> GetAllAsync();
}

public interface IItemRepository : IRepository<Item>
{
    /// <summary>Tracked item with its unit loaded (for stock changes).</summary>
    Task<Item?> GetWithUnitAsync(int id);

    /// <summary>Tracked items with units loaded, fetched in one round trip.</summary>
    Task<List<Item>> GetWithUnitByIdsAsync(IReadOnlyCollection<int> ids);

    /// <summary>Read-only list of active items, ordered by name.</summary>
    Task<List<Item>> GetActiveAsync();
}

public interface ICustomerRepository : IRepository<Customer>
{
    /// <summary>Read-only list, walk-in customer first, then by name.</summary>
    Task<List<Customer>> GetAllOrderedAsync();
}

public interface ISupplierRepository : IRepository<Supplier>
{
    Task<List<Supplier>> GetAllOrderedAsync();
}

public interface IPurchaseInvoiceRepository : IRepository<PurchaseInvoice>
{
    /// <summary>One page of bills (newest first) with supplier and lines loaded.</summary>
    Task<List<PurchaseInvoice>> SearchAsync(PurchaseInvoiceQuery query, int skip, int take);

    /// <summary>Count and totals over all bills matching the filters.</summary>
    Task<PurchaseSummary> SummarizeAsync(PurchaseInvoiceQuery query);
}

public interface ISalesInvoiceRepository : IRepository<SalesInvoice>
{
    /// <summary>One page of invoices (newest first) with customer and lines loaded.</summary>
    Task<List<SalesInvoice>> SearchAsync(SalesInvoiceQuery query, int skip, int take);

    /// <summary>Count and totals over all invoices matching the filters.</summary>
    Task<SalesSummary> SummarizeAsync(SalesInvoiceQuery query);
}

public interface ISalesReturnRepository : IRepository<SalesReturn>
{
    Task<List<SalesReturn>> GetAllWithDetailsAsync();
}

public interface IPurchaseReturnRepository : IRepository<PurchaseReturn>
{
    Task<List<PurchaseReturn>> GetAllWithDetailsAsync();
}

public interface IStockAdjustmentRepository : IRepository<StockAdjustment>
{
    Task<List<StockAdjustment>> GetAllWithItemAsync();
}

public interface IPaymentTransactionRepository : IRepository<PaymentTransaction>
{
    Task<List<PaymentTransaction>> GetByPartyTypeAsync(PartyType partyType);
}

public interface IExpenseRepository : IRepository<ShopExpense>
{
    Task<List<ShopExpense>> GetAllAsync();
}

public interface IDocumentSequenceRepository : IRepository<DocumentSequence>
{
    Task<DocumentSequence?> GetByKeyAsync(string sequenceKey);
}

public interface ICashBookRepository : IRepository<CashBookEntry>
{
    /// <summary>Entries matching the filters, oldest first (ledger order).</summary>
    Task<List<CashBookEntry>> SearchAsync(CashBookQuery query, int skip, int take);

    Task<int> CountAsync(CashBookQuery query);

    /// <summary>Money in / out per payment mode for entries on or after <paramref name="fromInclusive"/> and before <paramref name="toExclusive"/>.</summary>
    Task<List<CashBookModeTotal>> GetModeTotalsAsync(DateTime? fromInclusive, DateTime? toExclusive, PaymentMode? mode);

    /// <summary>Tracked entries created by one source record (used when that record is deleted).</summary>
    Task<List<CashBookEntry>> GetBySourceAsync(CashBookSource source, int sourceId);
}

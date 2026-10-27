using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Abstractions;

/// <summary>
/// One business operation = one unit of work (one DbContext behind the scenes).
/// Create it with <see cref="IUnitOfWorkFactory"/>, dispose it when the operation ends.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    IUnitOfMeasureRepository Units { get; }
    IItemRepository Items { get; }
    ICustomerRepository Customers { get; }
    ISupplierRepository Suppliers { get; }
    IPurchaseInvoiceRepository PurchaseInvoices { get; }
    ISalesInvoiceRepository SalesInvoices { get; }
    ISalesReturnRepository SalesReturns { get; }
    IPurchaseReturnRepository PurchaseReturns { get; }
    IStockAdjustmentRepository StockAdjustments { get; }
    IRepository<StockLedger> StockLedgers { get; }
    IPaymentTransactionRepository Payments { get; }
    IExpenseRepository Expenses { get; }
    IDocumentSequenceRepository DocumentSequences { get; }
    ICashBookRepository CashBook { get; }

    Task<int> SaveChangesAsync();
    Task<IUnitOfWorkTransaction> BeginTransactionAsync();
}

/// <summary>Disposing without committing rolls the transaction back.</summary>
public interface IUnitOfWorkTransaction : IAsyncDisposable
{
    Task CommitAsync();
}

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create();
}

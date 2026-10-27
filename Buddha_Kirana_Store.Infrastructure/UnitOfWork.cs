using Buddha_Kirana_Store.Abstractions;
using Buddha_Kirana_Store.Data.Repositories;
using Buddha_Kirana_Store.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Buddha_Kirana_Store.Data;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly KiranaDbContext _db;

    public UnitOfWork(KiranaDbContext db)
    {
        _db = db;

        Units = new UnitOfMeasureRepository(db);
        Items = new ItemRepository(db);
        Customers = new CustomerRepository(db);
        Suppliers = new SupplierRepository(db);
        PurchaseInvoices = new PurchaseInvoiceRepository(db);
        SalesInvoices = new SalesInvoiceRepository(db);
        SalesReturns = new SalesReturnRepository(db);
        PurchaseReturns = new PurchaseReturnRepository(db);
        StockAdjustments = new StockAdjustmentRepository(db);
        StockLedgers = new StockLedgerRepository(db);
        Payments = new PaymentTransactionRepository(db);
        Expenses = new ExpenseRepository(db);
        DocumentSequences = new DocumentSequenceRepository(db);
        CashBook = new CashBookRepository(db);
    }

    public IUnitOfMeasureRepository Units { get; }
    public IItemRepository Items { get; }
    public ICustomerRepository Customers { get; }
    public ISupplierRepository Suppliers { get; }
    public IPurchaseInvoiceRepository PurchaseInvoices { get; }
    public ISalesInvoiceRepository SalesInvoices { get; }
    public ISalesReturnRepository SalesReturns { get; }
    public IPurchaseReturnRepository PurchaseReturns { get; }
    public IStockAdjustmentRepository StockAdjustments { get; }
    public IRepository<StockLedger> StockLedgers { get; }
    public IPaymentTransactionRepository Payments { get; }
    public IExpenseRepository Expenses { get; }
    public IDocumentSequenceRepository DocumentSequences { get; }
    public ICashBookRepository CashBook { get; }

    public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();

    public async Task<IUnitOfWorkTransaction> BeginTransactionAsync() =>
        new EfUnitOfWorkTransaction(await _db.Database.BeginTransactionAsync());

    public ValueTask DisposeAsync() => _db.DisposeAsync();
}

internal sealed class EfUnitOfWorkTransaction(IDbContextTransaction transaction) : IUnitOfWorkTransaction
{
    public Task CommitAsync() => transaction.CommitAsync();

    public ValueTask DisposeAsync() => transaction.DisposeAsync();
}

internal sealed class UnitOfWorkFactory(IDbContextFactory<KiranaDbContext> dbFactory) : IUnitOfWorkFactory
{
    public IUnitOfWork Create() => new UnitOfWork(dbFactory.CreateDbContext());
}

internal sealed class DatabaseInitializer(IDbContextFactory<KiranaDbContext> dbFactory) : IDatabaseInitializer
{
    public async Task InitializeAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        await db.Database.MigrateAsync();
    }
}

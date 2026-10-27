using Buddha_Kirana_Store.Domain;
using Microsoft.EntityFrameworkCore;

namespace Buddha_Kirana_Store.Data;

/// <summary>
/// Pure persistence mapping. Every entity is configured in its own
/// <c>IEntityTypeConfiguration</c> class under Configurations/, so adding or changing
/// an entity never means editing this file.
/// </summary>
public class KiranaDbContext(DbContextOptions<KiranaDbContext> options) : DbContext(options)
{
    public DbSet<UnitOfMeasure> UnitsOfMeasure => Set<UnitOfMeasure>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<PurchaseInvoice> PurchaseInvoices => Set<PurchaseInvoice>();
    public DbSet<PurchaseInvoiceItem> PurchaseInvoiceItems => Set<PurchaseInvoiceItem>();
    public DbSet<SalesInvoice> SalesInvoices => Set<SalesInvoice>();
    public DbSet<SalesInvoiceItem> SalesInvoiceItems => Set<SalesInvoiceItem>();
    public DbSet<StockAdjustment> StockAdjustments => Set<StockAdjustment>();
    public DbSet<StockLedger> StockLedgers => Set<StockLedger>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<ShopExpense> ShopExpenses => Set<ShopExpense>();
    public DbSet<SalesReturn> SalesReturns => Set<SalesReturn>();
    public DbSet<SalesReturnItem> SalesReturnItems => Set<SalesReturnItem>();
    public DbSet<PurchaseReturn> PurchaseReturns => Set<PurchaseReturn>();
    public DbSet<PurchaseReturnItem> PurchaseReturnItems => Set<PurchaseReturnItem>();
    public DbSet<DocumentSequence> DocumentSequences => Set<DocumentSequence>();
    public DbSet<CashBookEntry> CashBookEntries => Set<CashBookEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(KiranaDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Money columns: numeric(18,2). Quantity columns override this to (18,3) in their configuration.
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);

        // Npgsql maps DateTime to "timestamptz" by default and rejects Local/Unspecified values.
        // This app stores shop-local date/times (DateTime.Now, date pickers), so use plain "timestamp".
        configurationBuilder.Properties<DateTime>().HaveColumnType("timestamp without time zone");
    }
}

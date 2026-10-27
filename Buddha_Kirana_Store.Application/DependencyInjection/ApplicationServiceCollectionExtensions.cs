using Buddha_Kirana_Store.Abstractions;
using Buddha_Kirana_Store.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    /// <summary>Registers every business service. Implementations stay internal to this assembly.</summary>
    public static IServiceCollection AddKiranaApplication(this IServiceCollection services)
    {
        // Shared building blocks (stateless)
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IStockService, StockService>();
        services.AddSingleton<IDocumentNumberGenerator, DocumentNumberGenerator>();
        services.AddSingleton<ICashBookRecorder, CashBookRecorder>();
        services.AddSingleton<IAdjustmentLossPolicy, DefaultAdjustmentLossPolicy>();

        // Extension points: add a new strategy / ledger here, nothing else changes.
        services.AddSingleton<IBillingStrategy, PerItemBillingStrategy>();
        services.AddSingleton<IBillingStrategy, LumpSumBillingStrategy>();
        services.AddSingleton<IPartyLedger, CustomerLedger>();
        services.AddSingleton<IPartyLedger, SupplierLedger>();

        // Use-case services the Razor pages depend on
        services.AddTransient<IItemService, ItemService>();
        services.AddTransient<IUnitOfMeasureService, UnitOfMeasureService>();
        services.AddTransient<ICustomerService, CustomerService>();
        services.AddTransient<ISupplierService, SupplierService>();
        services.AddTransient<IPartyPaymentService, PartyPaymentService>();
        services.AddTransient<IStockAdjustmentService, StockAdjustmentService>();
        services.AddTransient<IPurchaseService, PurchaseService>();
        services.AddTransient<IPurchaseReturnService, PurchaseReturnService>();
        services.AddTransient<ISalesService, SalesService>();
        services.AddTransient<ISalesReturnService, SalesReturnService>();
        services.AddTransient<IExpenseService, ExpenseService>();
        services.AddTransient<ICashBookService, CashBookService>();

        return services;
    }
}

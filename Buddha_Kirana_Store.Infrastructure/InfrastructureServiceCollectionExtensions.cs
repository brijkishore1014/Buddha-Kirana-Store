using Buddha_Kirana_Store.Abstractions;
using Buddha_Kirana_Store.Data;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    /// <summary>Registers PostgreSQL persistence. This is the only place that knows which database is used.</summary>
    public static IServiceCollection AddKiranaInfrastructure(this IServiceCollection services, string connectionString)
    {
        // Note: do not enable EnableRetryOnFailure() here - the use-case services open their own
        // transactions, which EF's retrying execution strategy does not allow.
        services.AddDbContextFactory<KiranaDbContext>(options => options.UseNpgsql(connectionString));

        services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
        services.AddSingleton<IDatabaseInitializer, DatabaseInitializer>();

        return services;
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Buddha_Kirana_Store.Data;

/// <summary>
/// Used only by the EF tools (dotnet ef migrations ...) so migrations can be created
/// from this class library without starting the MAUI app.
/// Set the KIRANA_DB environment variable to point the tools at a different server.
/// </summary>
public sealed class KiranaDbContextFactory : IDesignTimeDbContextFactory<KiranaDbContext>
{
    public KiranaDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("KIRANA_DB")
                               ?? "Host=localhost;Port=5432;Database=buddha_kirana;Username=postgres;Password=CHANGE_ME";

        var options = new DbContextOptionsBuilder<KiranaDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new KiranaDbContext(options);
    }
}

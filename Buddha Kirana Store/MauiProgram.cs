using System.Text.Json;
using Buddha_Kirana_Store.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

namespace Buddha_Kirana_Store;

public static class MauiProgram
{
    // Used when neither the KIRANA_DB environment variable nor an embedded appsettings.json provides one.
    // Replace CHANGE_ME, or set the KIRANA_DB environment variable so the password stays out of the app.
    private const string DefaultConnectionString =
        "Host=localhost;Port=5432;Database=buddha_kirana;Username=postgres;Password=CHANGE_ME";

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddMudServices();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Composition root: the only place that knows the concrete layers.
        builder.Services.AddKiranaInfrastructure(ResolveConnectionString());
        builder.Services.AddKiranaApplication();

        var app = builder.Build();
        InitializeDatabase(app);
        return app;
    }

    /// <summary>KIRANA_DB environment variable, then embedded appsettings.json, then the default above.</summary>
    private static string ResolveConnectionString()
    {
        var fromEnvironment = Environment.GetEnvironmentVariable("KIRANA_DB");
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
            return fromEnvironment;

        var assembly = typeof(MauiProgram).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith("appsettings.json", StringComparison.OrdinalIgnoreCase));

        if (resourceName is not null)
        {
            using var stream = assembly.GetManifestResourceStream(resourceName)!;
            using var document = JsonDocument.Parse(stream);

            if (document.RootElement.TryGetProperty("ConnectionStrings", out var connectionStrings) &&
                connectionStrings.TryGetProperty("KiranaDb", out var value) &&
                value.GetString() is { Length: > 0 } connectionString)
            {
                return connectionString;
            }
        }

        return DefaultConnectionString;
    }

    /// <summary>Applies pending EF migrations (creates the database on first run).</summary>
    private static void InitializeDatabase(MauiApp app)
    {

        try
        {
            var initializer = app.Services.GetRequiredService<IDatabaseInitializer>();

            // Task.Run avoids blocking on the UI synchronization context.
            Task.Run(initializer.InitializeAsync).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Could not connect to or update the PostgreSQL database: " + ex.GetBaseException().Message, ex);
        }
    }
}

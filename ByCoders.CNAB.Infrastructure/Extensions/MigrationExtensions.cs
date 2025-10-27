using ByCoders.CNAB.Infrastructure.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ByCoders.CNAB.Infrastructure.Extensions;

/// <summary>
/// Extensions for automatic migration application
/// </summary>
public static class MigrationExtensions
{
    /// <summary>
    /// Applies migrations automatically for both DbContexts on startup
    /// Works with generic IServiceProvider (API and Worker)
    /// </summary>
    public static void ApplyMigrations(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var loggerFactory = scopedServices.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("Migrations");

        try
        {
            logger.LogInformation("Starting migrations application...");

            // Migration for CNABFileDbContext
            var cnabFileContext = scopedServices.GetRequiredService<CNABFileDbContext>();
            logger.LogInformation("Applying migrations for CNABFileDbContext...");
            cnabFileContext.Database.Migrate();

            logger.LogInformation("CNABFileDbContext migrations applied successfully");

            logger.LogInformation("All migrations applied successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error applying migrations");
            throw;
        }
    }
}
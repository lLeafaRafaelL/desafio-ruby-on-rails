using ByCoders.CNAB.API.Configurations;
using ByCoders.CNAB.Domain.Files;
using ByCoders.CNAB.Domain.Transactions;
using ByCoders.CNAB.Infrastructure.Correlation;
using ByCoders.CNAB.Infrastructure.EntityFrameworkCore.Configurations;
using ByCoders.CNAB.Infrastructure.Repositories;
using ByCoders.CNAB.Infrastructure.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ByCoders.CNAB.Infrastructure.DI;

public static class InfraDependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddPostgresProvider(connectionString);

        services.AddScoped<ICorrelationService, CorrelationService>();

        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<ICNABFileRepository, CNABFileRepository>();

        services.AddScoped<IFileStorageService, FileStorageService>();

        services.AddHealthChecks()
                .AddHealthcheckPostgre(connectionString);

        services.AddMemoryCache();

        return services;
    }

    /// <summary>
    /// Configure Serlilog using appsettings
    /// </summary>
    /// <param name="builder"></param>
    public static void ConfigureSerilog(this IHostBuilder builder)
    {
        builder.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));
    }

    private static IHealthChecksBuilder AddHealthcheckPostgre(this IHealthChecksBuilder builder,
        string connstring, string? appname = null, TimeSpan? timeout = null, IEnumerable<string>? tags = null)
        {
            appname = string.IsNullOrWhiteSpace(appname) ? Environment.MachineName : appname;
            timeout = timeout ?? TimeSpan.FromSeconds(5);
            return builder.AddNpgSql(
                connstring,
                healthQuery: "SELECT 1",
                name: appname,
                tags: tags);
        }
}

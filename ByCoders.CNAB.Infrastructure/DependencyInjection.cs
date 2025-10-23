using ByCoders.CNAB.Domain.Transactions;
using ByCoders.CNAB.Infrastructure.EntityFrameworkCore.Configurations;
using ByCoders.CNAB.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ByCoders.CNAB.Infrastructure;

/// <summary>
/// Configura a injeção de dependências da camada Infrastructure
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adiciona os serviços da camada Infrastructure ao container de DI
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="connectionString">Connection string do PostgreSQL</param>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        // Configurar Serilog para logging no console
        ConfigureSerilog(services);

        // Configurar DbContext com PostgreSQL (usando configuração existente)
        services.AddPostgresProvider(connectionString);

        // Registrar repositórios
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        return services;
    }

    /// <summary>
    /// Configura o Serilog para logging no console
    /// </summary>
    private static void ConfigureSerilog(IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        // Adicionar Serilog como provider de logging
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog(Log.Logger, dispose: true);
        });
    }
}

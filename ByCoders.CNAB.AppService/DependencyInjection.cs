using ByCoders.CNAB.AppService.Transactions.CNAB.Import;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ByCoders.CNAB.AppService;

/// <summary>
/// Configura a injeção de dependências da camada AppService
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adiciona os serviços da camada AppService ao container de DI
    /// </summary>
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        // Registrar validators do FluentValidation
        services.AddValidatorsFromAssemblyContaining<CNABLineDataDtoValidator>();

        // Registrar parsers e factories
        services.AddScoped<CNABLineParser>();
        services.AddScoped<ITransactionFactory, TransactionFactory>();

        // Registrar handlers
        services.AddScoped<ImportCNABRequestHandler>();

        return services;
    }
}

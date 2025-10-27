using ByCoders.CNAB.Application.Files.CNAB.Parsers;
using ByCoders.CNAB.Application.Files.CNAB.Process;
using ByCoders.CNAB.Application.Transactions;
using ByCoders.CNAB.Core;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ByCoders.CNAB.Application.DI;

/// <summary>
/// Confirue Application Dependency Injection
/// </summary>
public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplications(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblies(new[] { typeof(ApplicationDependencyInjection).Assembly });

        services.AddScoped<ICNABLineParser, CNABLineParser>();
        services.AddScoped<IProcessCNABFileService, ProcessCNABFileService>();
        services.AddScoped<ITransactionFactory, TransactionFactory>();


        services.RegisterRequestHandlers();

        return services;
    }
}

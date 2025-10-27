using ByCoders.CNAB.Core.Validators;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ByCoders.CNAB.Core;

public static class RequestHandlerDependencyInjectionExtensions
{
    public static IServiceCollection RegisterRequestHandlers(this IServiceCollection services)
    {
        services.AddDependencies(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>), "*Application.dll")
            .AddDependencies(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDtoValidator<>), "*Application.dll");

        return services;
    }

    private static IServiceCollection AddDependencies(this IServiceCollection services, Func<Type, bool> filter, params string[] searchPatterns)
    {
        LoadAssemblies(searchPatterns);

        var types = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes().Where(t => !t.IsAbstract && t.GetInterfaces().Any(filter)))
            .ToList();

        foreach (var type in types)
        {
            var serviceType = type.GetInterfaces().FirstOrDefault(i => i.GetInterfaces().Any(filter))
                    ?? type.GetInterfaces().First(filter);

            services.AddScoped(serviceType, type);
        }

        return services;
    }

    private static void LoadAssemblies(params string[] assembliesToLoad)
    {
        if (assembliesToLoad is not null)
            foreach (var assembly in assembliesToLoad)
            {
                var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, assembly, SearchOption.AllDirectories);
                if (files is not null)
                    foreach (var file in files)
                    {
                        AppDomain.CurrentDomain.Load(Path.GetFileNameWithoutExtension(file));
                    }
            }
    }
}
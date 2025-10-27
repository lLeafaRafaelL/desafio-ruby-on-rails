using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ByCoders.CNAB.Infrastructure.EntityFrameworkCore.Configurations;

public static class DatabaseConfiguration
{
    public static void AddPostgresProvider(this IServiceCollection services, string connectionString)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyVersionBehavior", true);
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services
            .AddDbContext<CNABFileDbContext>(options => options
            .UseNpgsql(connectionString)
            .UseLowerCaseNamingConvention());
    }
}
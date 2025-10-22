using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Linq;

using Microsoft.EntityFrameworkCore.Metadata;

namespace ByCoders.CNAB.Infrastructure.EntityFrameworkCore.Configurations;

public static class DatabaseConfiguration
{
    public static void AddPostgresProvider(this IServiceCollection services, string connectionString)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyVersionBehavior", true);
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services
            .AddDbContext<TransactionDbContext>(options => options
            .UseNpgsql(
                connectionString)
            .UseLowerCaseNamingConvention());
    }
}
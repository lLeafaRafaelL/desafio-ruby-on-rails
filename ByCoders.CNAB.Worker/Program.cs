using ByCoders.CNAB.API.Configurations;
using ByCoders.CNAB.Application.DI;
using ByCoders.CNAB.Core.BackgroundServices;
using ByCoders.CNAB.Core.Prometheus;
using ByCoders.CNAB.Infrastructure.DI;
using ByCoders.CNAB.Infrastructure.Extensions;
using ByCoders.CNAB.Worker.Configurations;
using ByCoders.CNAB.Worker.Files;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Serilog;

IConfiguration configuration = new ConfigurationBuilder()
   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
   .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: false)
   .AddEnvironmentVariables()
   .AddCommandLine(args)
   .Build();


var builder = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
        webBuilder.Configure(app =>
        {
            app.UsePrometheus(options =>
            {
                options.EnableOpenMetrics = true;
            })
            .UseHealthChecks("/health/liveness", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            })
            .UseHealthChecks("/health/readiness", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        }));


builder.ConfigureServices((hostContext, services) =>
{
    services.Configure<HostOptions>(options =>
    {
        options.ServicesStartConcurrently = true;
        options.ServicesStopConcurrently = true;
    });

    var connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

    services
        .AddOptions<FileStorageConfiguration>()
        .Bind(hostContext.Configuration.GetSection("FileStorage"))
        .ValidateDataAnnotations()
        .ValidateOnStart();
    
    // Configure CNAB File Processor settings
    services
        .AddOptions<CNABFileProcessorConfiguration>()
        .Bind(hostContext.Configuration.GetSection("CNABFileProcessor"))
        .ValidateDataAnnotations()
        .ValidateOnStart();


    // Add application layers
    services
        .AddInfrastructure(connectionString)
        .AddApplications();

    builder.UseSerilog();

    // Em Program.cs do Worker
    services.AddScoped<CNABFileProcessor>();

    // Configurar IScoopedService
    services.AddScoped<IScoopedService>(provider =>
        new ScopedService(provider, sp => 
            sp.GetRequiredService<CNABFileProcessor>()));



    // Add Background Service for CNAB file processing
    services.AddHostedService<CNABFileProcessorBackgroundService>();
});

var host = builder.Build();

// Apply migrations automatically
host.Services.ApplyMigrations();

try
{
    Log.Information("Starting CNAB Worker Application");
    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Worker application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
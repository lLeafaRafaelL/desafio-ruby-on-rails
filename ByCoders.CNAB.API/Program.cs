using ByCoders.CNAB.API.Configurations;
using ByCoders.CNAB.API.Filters;
using ByCoders.CNAB.API.Middlewares;
using ByCoders.CNAB.Application.DI;
using ByCoders.CNAB.Core.Prometheus;
using ByCoders.CNAB.Infrastructure.DI;
using ByCoders.CNAB.Infrastructure.Extensions;
using HealthChecks.UI.Client;
using Serilog;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Configure connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

var storagePath = builder.Configuration["FileStorage:BasePath"] ?? "/app/storage";

// Add services to container
builder.Services.AddControllers(config =>
{ 
    config.Filters.Add<ExceptionFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CNAB API",
        Version = "v1",
        Description = "API for CNAB file upload and transaction queries"
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services
    .AddOptions<FileStorageConfiguration>()
    .Bind(builder.Configuration.GetSection("FileStorage"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Add application layers
builder.Services.AddInfrastructure(connectionString);
builder.Services.AddApplications();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Apply migrations automatically
app.Services.ApplyMigrations();

app.UseSwagger()
    .UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CNAB API v1");
        c.RoutePrefix = string.Empty; // Define Swagger na raiz "/"
    })
    .UseSerilogRequestLogging()
    .UseCors();

app.UseRouting();
app.MapControllers();

// Redirecionar raiz para Swagger
app.MapGet("/", () => Results.Redirect("/swagger"))
   .ExcludeFromDescription();

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

app.UseMiddleware<CorrelationMiddleware>();

try
{
    Log.Information("Starting CNAB API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
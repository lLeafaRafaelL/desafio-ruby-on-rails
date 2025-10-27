using ByCoders.CNAB.Infrastructure.Correlation;

namespace ByCoders.CNAB.API.Middlewares;

public class CorrelationMiddleware
{
    public const string CorrelationIdHeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICorrelationService correlation)
    {
        var correlationIdHeader = context.Request.Headers[CorrelationIdHeaderName].FirstOrDefault();

        var correlationValido = Guid.TryParse(correlationIdHeader, out Guid correlationId);

        if (!correlationValido)
        {
            correlationId = Guid.NewGuid();
        }

        correlation.SetCorrelationId(correlationId);

        await _next(context);
    }
}
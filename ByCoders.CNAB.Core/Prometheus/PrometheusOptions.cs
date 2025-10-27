using Prometheus.HttpMetrics;

namespace ByCoders.CNAB.Core.Prometheus;

public sealed record PrometheusOptions
{
    public string? MetricsRoute { get; set; }

    public bool EnableOpenMetrics { get; set; }

    public HttpMiddlewareExporterOptions? HttpMiddlewareExporterOptions { get; set; }

    public PrometheusOptions()
    {
        MetricsRoute = "/metrics";
        EnableOpenMetrics = true;
    }
}
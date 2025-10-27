using Microsoft.AspNetCore.Builder;
using Prometheus;

namespace ByCoders.CNAB.Core.Prometheus;

public static class PrometheusExtensions
{
    public static IApplicationBuilder UsePrometheus(this IApplicationBuilder app, Action<PrometheusOptions>? configure = null)
    {
        var prometheusOptions = new PrometheusOptions();
        configure?.Invoke(prometheusOptions);

        return app.UsePrometheus(prometheusOptions);
    }

    public static IApplicationBuilder UsePrometheus(this IApplicationBuilder app, PrometheusOptions options)
    {
        var prometheusOptions = options ?? new PrometheusOptions();

        app.UseMetricServer(x => x.EnableOpenMetrics = prometheusOptions.EnableOpenMetrics, prometheusOptions.MetricsRoute)
           .UseHttpMetrics(prometheusOptions.HttpMiddlewareExporterOptions);

        return app;
    }
}
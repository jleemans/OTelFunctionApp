using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OTelFunctionApp;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        
        var otelEndpoint = context.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? "http://localhost:4317";

        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService("OTelFunctionApp"))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource("OTelFunctionApp")
                    .AddOtlpExporter(o => o.Endpoint = new Uri(otelEndpoint));
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddMeter(Meters.Name)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddOtlpExporter(o => o.Endpoint = new Uri(otelEndpoint));
            });
    })
    .ConfigureLogging((context, logging) =>
    {
        var otelEndpoint = context.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? "http://localhost:4317";

        logging.ClearProviders();
        logging.AddOpenTelemetry(loggerOptions =>
        {
            loggerOptions.IncludeScopes = true;
            loggerOptions.ParseStateValues = true;
            loggerOptions.IncludeFormattedMessage = true;
            loggerOptions.AddOtlpExporter(exporterOptions => exporterOptions.Endpoint = new Uri(otelEndpoint));
        });
    })
    .Build();

await host.RunAsync();
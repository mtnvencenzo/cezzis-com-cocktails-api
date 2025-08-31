namespace Cocktails.Api.StartupExtensions;

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

internal static class OTelExtensions
{
    private readonly static string[] ExcludedOTelRoutes = ["/metrics", "/alive", "/health"];

    internal static IHostApplicationBuilder AddApplicationOpenTelemetry(this IHostApplicationBuilder builder, string serviceName)
    {
        AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);

        var otelCollectorEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
        var isLocal = builder.Environment.IsEnvironment("local");

        var traceProviderBuilder = Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
            .AddSource("Azure.Cosmos.Operation")
            .AddAspNetCoreInstrumentation((o) =>
            {
                o.Filter = (httpContext) =>
                {
                    if (ExcludedOTelRoutes.Contains(httpContext.Request.Path.Value, StringComparer.OrdinalIgnoreCase))
                    {
                        return false;
                    }

                    return true;
                };
            })
            .AddHttpClientInstrumentation()
            .AddGrpcClientInstrumentation();

        if (Uri.IsWellFormedUriString(otelCollectorEndpoint, UriKind.Absolute))
        {
            traceProviderBuilder.AddOtlpExporter(options =>
            {
                options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                options.ExportProcessorType = ExportProcessorType.Simple;
                options.Endpoint = new Uri(otelCollectorEndpoint);
            });
        }

        // We want to view all traces in development
        if (isLocal)
        {
            traceProviderBuilder.SetSampler(new AlwaysOnSampler());
            //traceProviderBuilder.AddConsoleExporter();
        }

        if (isLocal || Uri.IsWellFormedUriString(otelCollectorEndpoint, UriKind.Absolute))
        {
            traceProviderBuilder.Build();
        }

        var meterProviderBuilder = Sdk.CreateMeterProviderBuilder()
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
            .AddRuntimeInstrumentation()
            .AddAspNetCoreInstrumentation();

        if (Uri.IsWellFormedUriString(otelCollectorEndpoint, UriKind.Absolute))
        {
            meterProviderBuilder.AddOtlpExporter(options =>
            {
                options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                options.ExportProcessorType = ExportProcessorType.Simple;
                options.Endpoint = new Uri(otelCollectorEndpoint);
            });
        }

        if (Uri.IsWellFormedUriString(otelCollectorEndpoint, UriKind.Absolute))
        {
            meterProviderBuilder.Build();
        }

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddOpenTelemetry(logging =>
            {
                logging.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName));
                logging.IncludeFormattedMessage = false;
                logging.IncludeScopes = false;

                if (Uri.IsWellFormedUriString(otelCollectorEndpoint, UriKind.Absolute))
                {
                    logging.AddOtlpExporter(options =>
                    {
                        options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                        options.ExportProcessorType = ExportProcessorType.Simple;
                        options.Endpoint = new Uri(otelCollectorEndpoint);
                    });
                }

                if (isLocal)
                {
                    logging.AddConsoleExporter();
                }
            });
        });

        return builder;
    }
}

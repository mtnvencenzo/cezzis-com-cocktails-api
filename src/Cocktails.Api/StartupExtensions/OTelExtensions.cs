namespace Cocktails.Api.StartupExtensions;

using Cezzi.OTel;
using Cocktails.Api.Application.Behaviors.ProbeTelemetry;
using Confluent.Kafka.Extensions.OpenTelemetry;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

internal static class OTelExtensions
{
    private static readonly HashSet<string> SuppressedHttpPaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "/v1.0/healthz/outbound"
    };

    internal static IHostApplicationBuilder AddOTelTelemetry(this IHostApplicationBuilder builder)
    {
        AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);

        builder.Services
            .AddHttpClient("OtlpMetricExporter")
            .RemoveAllLoggers();
        builder.Services
            .AddHttpClient("OtlpTraceExporter")
            .RemoveAllLoggers();

        builder.Services.Configure<HttpClientTraceInstrumentationOptions>(options =>
        {
            options.FilterHttpRequestMessage = (httpRequestMessage) =>
            {
                var path = httpRequestMessage.RequestUri?.AbsolutePath;
                return path == null || !SuppressedHttpPaths.Contains(path);
            };
        });

        builder.AddApplicationOpenTelemetry(
            configureTracing: (t) =>
            {
                t.AddConfluentKafkaInstrumentation();

                return t
                    .AddConfluentKafkaInstrumentation()
                    .AddSource(
                        "Azure.Core",
                        "Azure.Identity",
                        "Microsoft.Azure.Cosmos",
                        "Azure.Cosmos.Client",
                        "Azure.Storage.Blobs",
                        "Azure.Search.Documents"
                    );
            },
            configureResource: (resourceBuilder) =>
            {
                return resourceBuilder
                    .WithElasticApm(builder.Environment.EnvironmentName)
                    .AddAttributes(new Dictionary<string, object>
                    {
                        ["app.unit"] = "cocktails",
                        ["app.product"] = "cezzis.com",
                        ["app.product_segment"] = "backend",
                        ["app.name"] = "cezzis-com-cocktails-api",
                        ["app.class"] = "api",
                        ["app.env"] = builder.Environment.EnvironmentName?.ToLowerInvariant() ?? "unknown"
                    });
            }
        );

        // Suppress logs from all providers during health probe requests.
        // Uses IHttpContextAccessor (via ProbeRequestContext) so it catches
        // hosting-level "Request starting" / "Request finished" logs too.
        builder.Logging.AddFilter((category, level) =>
        {
            // Always allow Error logs
            if (level >= LogLevel.Error)
            {
                return true;
            }

            // Suppress probe logs for non-error levels
            if (ProbeRequestContext.IsProbeRequest)
            {
                return false;
            }

            return true;
        });

        // Suppress probe logs and enforce minimum Information level for the OTel provider.
        // Provider-specific filters take precedence over the global filter above,
        // so the probe check must be duplicated here.
        builder.Logging.AddFilter<OpenTelemetryLoggerProvider>((category, level) =>
        {
            // Always allow Error logs
            if (level >= LogLevel.Error)
            {
                return true;
            }

            // Suppress probe logs for non-error levels
            if (ProbeRequestContext.IsProbeRequest)
            {
                return false;
            }

            return level >= LogLevel.Information;
        });

        return builder;
    }
}
namespace Cocktails.Api.StartupExtensions;

using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Trace;

internal static class ServiceDefaultsExtensions
{
    private readonly static string[] ExcludedOTelRoutes = ["/metrics", "/alive", "/health", "/api/v1/health/ping"];

    internal static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        builder.AddBasicServiceDefaults();

        builder.Services.AddHttpCors(builder.Configuration);

        builder.Services.ConfigureJsonSerialization();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler();

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        builder.Services.AddProblemDetails();

        return builder;
    }

    private static IHostApplicationBuilder AddBasicServiceDefaults(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOptions();
        builder.Services.AddHttpClient();
        builder.Services.AddHttpContextAccessor();

        // TODO: Is this needed
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        // Default health checks assume the event bus and self health checks
        builder.AddDefaultHealthChecks();

        builder.AddApplicationOpenTelemetry();

        return builder;
    }

    private static IHostApplicationBuilder AddApplicationOpenTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Services.AddLogging();

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;

            if (builder.Environment.IsEnvironment("local"))
            {
                //logging.AddConsoleExporter();
            }
        });

        var openTelemetryBuilder = builder.Services
            .AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                if (builder.Environment.IsEnvironment("local"))
                {
                    // We want to view all traces in development
                    tracing.SetSampler(new AlwaysOnSampler());
                }

                tracing
                    .AddAspNetCoreInstrumentation((o) => o.Filter = (httpContext) =>
                    {
                        if (ExcludedOTelRoutes.Contains(httpContext.Request.Path.Value, StringComparer.OrdinalIgnoreCase))
                        {
                            return false;
                        }

                        return true;
                    })
                    .AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation();
            });

        if (!builder.Environment.IsEnvironment("local") && !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
        {
            openTelemetryBuilder.UseAzureMonitor();
        }

        return builder;
    }

    private static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
    {
        // Add a default liveness check to ensure app is responsive
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }
}

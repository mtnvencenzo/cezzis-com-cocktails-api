namespace Cocktails.Api.StartupExtensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

internal static class ServiceDefaultsExtensions
{
    internal static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        builder.AddBasicServiceDefaults();

        builder.AddOTelTelemetry();

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

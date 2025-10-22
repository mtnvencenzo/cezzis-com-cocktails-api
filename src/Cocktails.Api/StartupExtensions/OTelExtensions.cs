namespace Cocktails.Api.StartupExtensions;

using Cezzi.OTel;
using OpenTelemetry.Resources;

internal static class OTelExtensions
{
    internal static IHostApplicationBuilder AddOTelTelemetry(this IHostApplicationBuilder builder)
    {
        AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);

        builder.AddApplicationOpenTelemetry(
            configureTracing: (t) =>
            {
                return t.AddSource(
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

        return builder;
    }
}
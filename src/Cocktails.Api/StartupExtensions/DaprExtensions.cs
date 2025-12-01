namespace Cocktails.Api.StartupExtensions;

using Cocktails.Api.Domain.Config;
using Microsoft.Extensions.Options;

internal static class DaprExtensions
{
    internal static IServiceCollection AddDaprClient(this IServiceCollection services)
    {

        // Register the dapr client
        services.AddDaprClient((sp, dapr) =>
        {
            var config = sp.GetRequiredService<IOptions<DaprConfig>>().Value;

            if (!string.IsNullOrWhiteSpace(config.HttpEndpoint))
            {
                dapr.UseHttpEndpoint(config.HttpEndpoint);
            }

            if (!string.IsNullOrWhiteSpace(config.GrpcEndpoint))
            {
                dapr.UseGrpcEndpoint(config.GrpcEndpoint);
            }
        });
        return services;
    }
}

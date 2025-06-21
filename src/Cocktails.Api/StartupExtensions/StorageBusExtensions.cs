namespace Cocktails.Api.StartupExtensions;

using Cocktails.Api.Domain.Config;
using Cocktails.Api.Domain.Services;
using Cocktails.Api.Infrastructure.Services;

internal static class StorageBusExtensions
{
    internal static IServiceCollection AddStorageBus(this IServiceCollection services)
    {
        services.AddTransient<IStorageBus, DaprStorageBus>();
        return services;
    }
}

namespace Cocktails.Api.StartupExtensions;

using Cocktails.Api.Domain.Config;
using Cocktails.Api.Domain.Services;
using Cocktails.Api.Infrastructure.Services;

internal static class EventBusExtensions
{
    internal static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PubSubConfig>(configuration.GetSection(PubSubConfig.SectionName));
        services.AddTransient<IEventBus, DaprEventBus>();
        return services;
    }
}

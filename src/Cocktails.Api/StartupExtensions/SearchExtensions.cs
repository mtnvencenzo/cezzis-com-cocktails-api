namespace Cocktails.Api.StartupExtensions;

using Cocktails.Api.Domain.Config;
using Cocktails.Api.Infrastructure.Services;
using Microsoft.Extensions.Options;

internal static class SearchExtensions
{
    internal static IServiceCollection AddSearchClient(this IServiceCollection services)
    {
        services.AddSingleton<ISearchClient, AzSearchClient>((sp) =>
        {
            var searchConfig = sp.GetRequiredService<IOptions<SearchConfig>>().Value;

            return new AzSearchClient(
                endpoint: searchConfig.Endpoint,
                indexName: searchConfig.IndexName,
                key: searchConfig.QueryKey
            );
        });

        return services;
    }
}
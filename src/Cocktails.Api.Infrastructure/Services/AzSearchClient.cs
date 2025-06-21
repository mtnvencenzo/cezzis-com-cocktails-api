namespace Cocktails.Api.Infrastructure.Services;

using Azure.Search.Documents;
using Azure.Identity;
using Azure.Search.Documents.Models;
using Azure.Core;
using Azure;
using Cocktails.Api.Domain.Aggregates.CocktailAggregate;

public class AzSearchClient : ISearchClient
{
    private readonly SearchClient searchClient;

    public AzSearchClient(Uri endpoint, string indexName, string key = null)
    {
        var options = new SearchClientOptions();

        if (!string.IsNullOrWhiteSpace(key))
        {
            var credential = new AzureKeyCredential(key);

            this.searchClient = new SearchClient(
                endpoint: endpoint,
                indexName: indexName,
                credential: credential,
                options: options);
        }
        else
        {
            this.searchClient = new SearchClient(
                endpoint: endpoint,
                indexName: indexName,
                tokenCredential: new DefaultAzureCredential(),
                options: new SearchClientOptions());
        }
    }

    public async Task<List<Cocktail>> SearchAsync(List<Cocktail> cocktails, string query, int skip = 0, int take = 20, CancellationToken cancellationToken = default)
    {
        var options = new SearchOptions
        {
            QueryType = SearchQueryType.Full,
            SearchMode = SearchMode.Any,
            IncludeTotalCount = true
        };

        options.Select.Add("id");

        options.SearchFields.Add("TitlePrefix");
        options.SearchFields.Add("TitleSuffix");
        options.SearchFields.Add("Description");
        options.SearchFields.Add("DescriptiveTitle");
        options.SearchFields.Add("Ingredients/Name");

        options.OrderBy.Add("search.score() desc");
        options.Skip = skip;

        var searchQuery = query
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Length > 8 ? $"{x}~2" : x.Length > 5 ? $"{x}~1" : $"{x}*")
            .Aggregate((current, next) => $"{current} {next}");

        var response = await this.searchClient.SearchAsync<SearchDocument>(searchText: searchQuery, options: options, cancellationToken: cancellationToken);

        var results = response.Value.GetResultsAsync();
        var filtersCocktails = new List<Cocktail>();
        var forceBreak = false;

        await foreach (var page in results.AsPages())
        {
            foreach (var item in page.Values)
            {
                if (item.Score <= 2.0)
                {
                    continue;
                }

                var id = item.Document.GetString("id");

                var matching = cocktails.FirstOrDefault(c => c.Id == id);

                if (matching != null)
                {
                    filtersCocktails.Add(matching);
                }

                if (filtersCocktails.Count >= take)
                {
                    forceBreak = true;
                    break;
                }
            }

            if (forceBreak)
            {
                break;
            }
        }

        return filtersCocktails;
    }
}
namespace Cocktails.Api.Infrastructure.Services;

using Azure.Search.Documents.Models;
using Cocktails.Api.Domain.Aggregates.CocktailAggregate;

public interface ISearchClient
{
    Task<List<Cocktail>> SearchAsync(List<Cocktail> cocktails, string query, int skip = 0, int take = 20, CancellationToken cancellationToken = default);
}

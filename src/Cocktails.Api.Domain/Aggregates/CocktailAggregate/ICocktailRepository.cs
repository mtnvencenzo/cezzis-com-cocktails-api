namespace Cocktails.Api.Domain.Aggregates.CocktailAggregate;

using Cocktails.Api.Domain.Aggregates.AccountAggregate;
using Cocktails.Api.Domain.Common;

public interface ICocktailRepository : IRepository<Cocktail>, IReadonlyRepository<Cocktail>
{
    IQueryable<Cocktail> CachedItems { get; }

    Cocktail Add(Cocktail cocktail);

    Task<Cocktail> GetAsync(string id, CancellationToken cancellationToken);

    void Update(Cocktail cocktail);

    void ClearCache();

    void UpdateCache(Cocktail cocktail);
}

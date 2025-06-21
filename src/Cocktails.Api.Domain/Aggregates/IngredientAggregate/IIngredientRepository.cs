namespace Cocktails.Api.Domain.Aggregates.IngredientAggregate;

using Cocktails.Api.Domain.Aggregates.AccountAggregate;
using Cocktails.Api.Domain.Common;

public interface IIngredientRepository : IRepository<Ingredient>, IReadonlyRepository<Ingredient>
{
    IQueryable<Ingredient> CachedItems { get; }

    Ingredient Add(Ingredient ingredient);

    Task<Ingredient> GetAsync(string id, CancellationToken cancellationToken);

    void Update(Ingredient ingredient);

    void ClearCache();

    void UpdateCache(Ingredient ingredient);

    void Delete(Ingredient ingredient);
}

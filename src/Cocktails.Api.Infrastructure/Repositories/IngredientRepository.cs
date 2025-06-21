namespace Cocktails.Api.Infrastructure.Repositories;

using Cocktails.Api.Domain.Aggregates.IngredientAggregate;
using Cocktails.Api.Domain.Common;
using Cocktails.Api.Infrastructure.Resources.Ingredients;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class IngredientRepository(CocktailDbContext dbContext) : IIngredientRepository
{
    private readonly static Lock loadSync = new();
    private static List<Ingredient> cachedIngredients;

    public IUnitOfWork UnitOfWork => dbContext;

    public IQueryable<Ingredient> CachedItems
    {
        get
        {
            if (cachedIngredients == null)
            {
                using (loadSync.EnterScope())
                {
                    if (cachedIngredients == null)
                    {
                        cachedIngredients ??= Task.Run(() => dbContext.Ingredients.AsNoTracking().ToListAsync()).Result;
                    }
                }
            }

            return cachedIngredients.AsQueryable();
        }
    }

    public IQueryable<Ingredient> Items => dbContext.Ingredients;

    public Ingredient Add(Ingredient ingredient) => dbContext.Ingredients.Add(ingredient).Entity;

    public async Task<Ingredient> GetAsync(string ingredientId, CancellationToken cancellationToken) => await dbContext.Ingredients
        .WithPartitionKey(ingredientId)
        .FirstOrDefaultAsync(x => x.Id == ingredientId, cancellationToken);

    public void Update(Ingredient ingredient) => dbContext.Entry(ingredient).State = EntityState.Modified;

    public void Delete(Ingredient ingredient) => dbContext.Ingredients.Remove(ingredient);

    public void ClearCache() => cachedIngredients = null;

    public void UpdateCache(Ingredient ingredient)
    {
        if (cachedIngredients == null)
        {
            return;
        }

        using (loadSync.EnterScope())
        {
            var existing = cachedIngredients.FirstOrDefault(x => x.Id == ingredient.Id);

            if (existing != null)
            {
                var index = cachedIngredients.IndexOf(existing);
                cachedIngredients.Remove(existing);
                cachedIngredients.Insert(index, ingredient);
            }
            else
            {
                cachedIngredients.Add(ingredient);
            }
        }
    }
}
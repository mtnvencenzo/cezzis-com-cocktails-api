namespace Cocktails.Api.Infrastructure.Repositories;

using Cezzi.Applications.Extensions;
using Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using Cocktails.Api.Domain.Aggregates.IngredientAggregate;
using Cocktails.Api.Domain.Common;
using Cocktails.Api.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class CocktailRepository(CocktailDbContext dbContext) : ICocktailRepository
{
    private readonly static Lock loadSync = new();
    private static List<Cocktail> cachedCocktails;

    public IUnitOfWork UnitOfWork => dbContext;

    public IQueryable<Cocktail> CachedItems
    {
        get
        {
            if (cachedCocktails == null)
            {
                using (loadSync.EnterScope())
                {
                    if (cachedCocktails == null)
                    {
                        cachedCocktails ??= Task.Run(() => dbContext.Cocktails.AsNoTracking().ToListAsync()).Result;
                        cachedCocktails = [.. cachedCocktails.OrderBy(x => x.Title)];
                    }
                }
            }

            return cachedCocktails.AsQueryable();
        }
    }

    public IQueryable<Cocktail> Items => dbContext.Cocktails;

    public Cocktail Add(Cocktail cocktail) => dbContext.Cocktails.Add(cocktail).Entity;

    public async Task<Cocktail> GetAsync(string cocktailId, CancellationToken cancellationToken)
    {
        var cocktail = await dbContext.Cocktails
            .WithPartitionKey(cocktailId)
            .FirstOrDefaultAsync(x => x.Id == cocktailId, cancellationToken);

        return cocktail;
    }

    public void Update(Cocktail cocktail) => dbContext.Entry(cocktail).State = EntityState.Modified;

    public void ClearCache() => cachedCocktails = null;

    public void UpdateCache(Cocktail cocktail)
    {
        if (cachedCocktails == null)
        {
            return;
        }

        using (loadSync.EnterScope())
        {
            var existing = cachedCocktails.FirstOrDefault(x => x.Id == cocktail.Id);

            if (existing != null)
            {
                var index = cachedCocktails.IndexOf(existing);
                cachedCocktails.Remove(existing);
                cachedCocktails.Insert(index, cocktail);
            }
            else
            {
                cachedCocktails.Add(cocktail);
            }
        }
    }
}
namespace Cocktails.Api.Infrastructure.Repositories;

using Cocktails.Api.Domain.Aggregates.AccountAggregate;
using Cocktails.Api.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class AccountCocktailRatingsRepository(AccountDbContext dbContext) : IAccountCocktailRatingsRepository
{
    public virtual IUnitOfWork UnitOfWork => dbContext;

    public IQueryable<AccountCocktailRatings> Items => dbContext.CocktailRatings;

    public virtual AccountCocktailRatings Add(AccountCocktailRatings ratings) => dbContext.CocktailRatings.Add(ratings).Entity;

    public async Task<AccountCocktailRatings> GetAsync(string accountId, CancellationToken cancellationToken) => await dbContext.CocktailRatings.FirstOrDefaultAsync(x => x.Id == accountId, cancellationToken);

    public virtual void Update(AccountCocktailRatings ratings) => dbContext.Entry(ratings).State = EntityState.Modified;
}
namespace Cocktails.Api.Infrastructure.Repositories;

using Cezzi.Applications;
using Cocktails.Api.Domain.Aggregates.AccountAggregate;
using Cocktails.Api.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

public class AccountCocktailRatingsRepository(AccountDbContext dbContext) : IAccountCocktailRatingsRepository
{
    public IUnitOfWork UnitOfWork => dbContext;

    public IQueryable<AccountCocktailRatings> Items => dbContext.CocktailRatings;

    public AccountCocktailRatings Add(AccountCocktailRatings ratings) => dbContext.CocktailRatings.Add(ratings).Entity;

    public async Task<AccountCocktailRatings> GetAsync(string accountId, CancellationToken cancellationToken) => await dbContext.CocktailRatings.FirstOrDefaultAsync(x => x.Id == accountId, cancellationToken);

    public void Update(AccountCocktailRatings ratings) => dbContext.Entry(ratings).State = EntityState.Modified;
}
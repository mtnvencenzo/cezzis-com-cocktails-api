namespace Cocktails.Api.Domain.Aggregates.AccountAggregate;

using Cocktails.Api.Domain.Aggregates.AccountAggregate;
using Cocktails.Api.Domain.Common;
using System.Security.Claims;

public interface IAccountCocktailRatingsRepository : IRepository<AccountCocktailRatings>, IReadonlyRepository<AccountCocktailRatings>
{
    AccountCocktailRatings Add(AccountCocktailRatings ratings);

    Task<AccountCocktailRatings> GetAsync(string id, CancellationToken cancellationToken);

    void Update(AccountCocktailRatings ratings);
}

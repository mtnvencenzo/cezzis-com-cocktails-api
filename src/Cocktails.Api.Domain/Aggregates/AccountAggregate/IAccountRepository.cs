namespace Cocktails.Api.Domain.Aggregates.AccountAggregate;

using Cocktails.Api.Domain.Aggregates.AccountAggregate;
using Cocktails.Api.Domain.Common;
using System.Security.Claims;

public interface IAccountRepository : IRepository<Account>, IReadonlyRepository<Account>
{
    Account Add(Account account);

    Task<Account> GetAsync(string id, CancellationToken cancellationToken);

    void Update(Account account);

    Task<Account> GetOrCreateLocalAccountFromIdentity(ClaimsIdentity claimsIdentity, CancellationToken cancellationToken);
}

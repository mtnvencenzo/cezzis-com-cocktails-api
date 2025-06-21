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

public class AccountRepository(AccountDbContext dbContext) : IAccountRepository
{
    public IUnitOfWork UnitOfWork => dbContext;

    public IQueryable<Account> Items => dbContext.Accounts;

    public Account Add(Account account) => dbContext.Accounts.Add(account).Entity;

    public async Task<Account> GetAsync(string accountId, CancellationToken cancellationToken) => await dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == accountId, cancellationToken);

    public void Update(Account account) => dbContext.Entry(account).State = EntityState.Modified;

    public async Task<Account> GetOrCreateLocalAccountFromIdentity(ClaimsIdentity claimsIdentity, CancellationToken cancellationToken)
    {
        var claimsAccount = new ClaimsAccount(claimsIdentity);

        var account = await this.Items
            .WithPartitionKey(claimsAccount.SubjectId)
            .FirstOrDefaultAsync(x => x.SubjectId == claimsAccount.SubjectId, cancellationToken);

        if (account == null)
        {
            account = this.Add(new Account(claimsAccount));

            _ = await this.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }

        Guard.Equals(account?.SubjectId, claimsAccount.SubjectId);

        return account;
    }
}
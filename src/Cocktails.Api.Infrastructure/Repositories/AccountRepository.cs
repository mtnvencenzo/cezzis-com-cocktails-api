namespace Cocktails.Api.Infrastructure.Repositories;

using Cezzi.Applications;
using Cocktails.Api.Domain.Aggregates.AccountAggregate;
using Cocktails.Api.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

public class AccountRepository(AccountDbContext dbContext) : IAccountRepository
{
    public virtual IUnitOfWork UnitOfWork => dbContext;

    public IQueryable<Account> Items => dbContext.Accounts;

    public virtual Account Add(Account account) => this.AddInternal(account);

    public async Task<Account> GetAsync(string accountId, CancellationToken cancellationToken) => await dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == accountId, cancellationToken);

    public virtual void Update(Account account) => dbContext.Entry(account).State = EntityState.Modified;

    public async Task<(Account profile, bool created)> GetOrCreateLocalAccountFromIdentity(ClaimsIdentity claimsIdentity, CancellationToken cancellationToken)
    {
        var account = await this.GetLocalAccountFromIdentity(
            claimsIdentity: claimsIdentity,
            cancellationToken: cancellationToken);

        if (account != null)
        {
            Guard.Equals(account?.SubjectId, account.SubjectId);
            return (account, false);
        }

        account = this.Add(new Account(new ClaimsAccount(claimsIdentity)));

        _ = await this.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        Guard.Equals(account?.SubjectId, account.SubjectId);

        return (account, true);
    }

    public async Task<Account> GetLocalAccountFromIdentity(ClaimsIdentity claimsIdentity, CancellationToken cancellationToken)
    {
        var claimsAccount = new ClaimsAccount(claimsIdentity);

        var account = await this.Items
            .WithPartitionKey(claimsAccount.SubjectId)
            .FirstOrDefaultAsync(x => x.SubjectId == claimsAccount.SubjectId, cancellationToken);

        if (account != null)
        {
            Guard.Equals(account.SubjectId, claimsAccount.SubjectId);
        }

        return account;
    }

    private Account AddInternal(Account account) => dbContext.Accounts.Add(account).Entity;
}
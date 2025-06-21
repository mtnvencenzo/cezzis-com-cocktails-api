namespace Cocktails.Api.Infrastructure;

using Cocktails.Api.Domain.Aggregates.AccountAggregate;
using Cocktails.Api.Domain.Common;
using Cocktails.Api.Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

public class AccountDbContext : DbContext, IUnitOfWork
{
    private readonly IMediator mediator;

    public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options) { }

    public AccountDbContext(DbContextOptions<AccountDbContext> options, IMediator mediator) : base(options)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly, (t) => t.GetInterfaces().Contains(typeof(IAccountContextEntityConfiguration)));
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        // Dispatch Domain Events collection.
        // Choices:
        // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
        // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
        // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
        // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
        await this.mediator.DispatchDomainEventsAsync(this);

        // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
        // performed through the DbContext will be committed
        _ = await base.SaveChangesAsync(cancellationToken);

        return true;
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AccountCocktailRatings> CocktailRatings { get; set; }
}

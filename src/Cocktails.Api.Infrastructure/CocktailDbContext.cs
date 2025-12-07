namespace Cocktails.Api.Infrastructure;

using Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using Cocktails.Api.Domain.Aggregates.IngredientAggregate;
using Cocktails.Api.Domain.Common;
using Cocktails.Api.Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Cocktails.Api.Domain.Config;

public class CocktailDbContext : DbContext, IUnitOfWork
{
    private readonly IMediator mediator;
    private readonly IOptions<CosmosDbConfig> cosmosDbConfig;

    public CocktailDbContext() { }

    public CocktailDbContext(DbContextOptions<CocktailDbContext> options) : base(options) { }

    public CocktailDbContext(DbContextOptions<CocktailDbContext> options, IMediator mediator, IOptions<CosmosDbConfig> cosmosDbConfig) : base(options)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this.cosmosDbConfig = cosmosDbConfig ?? throw new ArgumentNullException(nameof(cosmosDbConfig));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new CocktailEntityTypeConfiguration(this.cosmosDbConfig));
        modelBuilder.ApplyConfiguration(new IngredientEntityTypeConfiguration(this.cosmosDbConfig));
    }

    public virtual async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
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

    public virtual DbSet<Cocktail> Cocktails { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }
}

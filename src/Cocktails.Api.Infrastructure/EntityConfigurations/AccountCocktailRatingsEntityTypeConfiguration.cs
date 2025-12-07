namespace Cocktails.Api.Infrastructure.EntityConfigurations;

using Cocktails.Api.Domain.Aggregates.AccountAggregate;
using Cocktails.Api.Domain.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;

public class AccountCocktailRatingsEntityTypeConfiguration(IOptions<CosmosDbConfig> cosmosDbConfig) : IEntityTypeConfiguration<AccountCocktailRatings>, IAccountContextEntityConfiguration
{
    public void Configure(EntityTypeBuilder<AccountCocktailRatings> builder)
    {
        var containerName = cosmosDbConfig.Value.AccountsContainerName
            ?? throw new ArgumentNullException("CosmosDb__AccountsContainerName");

        builder
            .ToContainer(containerName)
            .HasPartitionKey(x => x.SubjectId)
            .ApplyCamelCasingNamingStrategry()
            .HasKey(x => x.Id);

        builder.Property(x => x.ETag).IsETagConcurrency();

        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.HasDiscriminator(x => x.Discriminator).HasValue<AccountCocktailRatings>("account-cocktail-ratings");

        builder.OwnsMany(
            x => x.Ratings,
            x =>
            {
                x.Property(c => c.Id).ValueGeneratedOnAdd();
                x.ApplyCamelCasingNamingStrategry();
                x.WithOwner();
            });
    }
}

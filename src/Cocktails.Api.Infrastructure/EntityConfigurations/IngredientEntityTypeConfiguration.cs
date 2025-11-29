namespace Cocktails.Api.Infrastructure.EntityConfigurations;

using Cocktails.Api.Domain.Aggregates.IngredientAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class IngredientEntityTypeConfiguration : IEntityTypeConfiguration<Ingredient>, ICocktailContextEntityConfiguration
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        var containerName = Environment.GetEnvironmentVariable("CosmosDb__IngredientsContainerName")
            ?? throw new ArgumentNullException("CosmosDb__IngredientsContainerName");

        builder
            .ToContainer(containerName)
            .HasPartitionKey(x => x.Id)
            .HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.ETag).IsETagConcurrency();

        builder.Property(x => x.Types).ToJsonProperty("types");

        builder.Property(x => x.Applications).ToJsonProperty("applications");

        builder.HasDiscriminator(x => x.Discriminator).HasValue("ingredient");

        builder.Ignore(x => x.DomainEvents);
    }
}

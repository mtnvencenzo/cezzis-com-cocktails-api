namespace Cocktails.Api.Infrastructure.EntityConfigurations;

using Cocktails.Api.Domain.Aggregates.AccountAggregate;
using Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using Cocktails.Api.Domain.Aggregates.IngredientAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using System.Text.Json.Serialization;

public class IngredientEntityTypeConfiguration : IEntityTypeConfiguration<Ingredient>, ICocktailContextEntityConfiguration
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder
            .ToContainer("cocktails-ingredient")
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

namespace Cocktails.Api.Infrastructure.EntityConfigurations;

using Cocktails.Api.Domain.Aggregates.AccountAggregate;
using Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using System.Text.Json.Serialization;

public class CocktailEntityTypeConfiguration : IEntityTypeConfiguration<Cocktail>, ICocktailContextEntityConfiguration
{
    public void Configure(EntityTypeBuilder<Cocktail> builder)
    {
        builder
            .ToContainer("cocktails-cocktail")
            .HasPartitionKey(x => x.Id)
            .HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.ETag).IsETagConcurrency();

        builder.Property(x => x.SearchableTitles).ToJsonProperty("searchableTitles");

        builder.Property(x => x.Eras).ToJsonProperty("eras");

        builder.Property(x => x.Glassware).ToJsonProperty("glassware");

        builder.HasDiscriminator(x => x.Discriminator).HasValue("cocktail");

        builder.Ignore(x => x.DomainEvents);
    }
}

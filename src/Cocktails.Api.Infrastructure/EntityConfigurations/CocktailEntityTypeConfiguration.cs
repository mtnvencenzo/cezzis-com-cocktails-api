namespace Cocktails.Api.Infrastructure.EntityConfigurations;

using Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using Cocktails.Api.Domain.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;

public class CocktailEntityTypeConfiguration(IOptions<CosmosDbConfig> cosmosDbConfig) : IEntityTypeConfiguration<Cocktail>
{
    public void Configure(EntityTypeBuilder<Cocktail> builder)
    {
        var containerName = cosmosDbConfig.Value.CocktailsContainerName
            ?? throw new ArgumentNullException("CosmosDb__CocktailsContainerName");

        builder
            .ToContainer(containerName)
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

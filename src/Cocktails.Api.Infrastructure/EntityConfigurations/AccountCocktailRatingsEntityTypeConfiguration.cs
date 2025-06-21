namespace Cocktails.Api.Infrastructure.EntityConfigurations;

using Cocktails.Api.Domain.Aggregates.AccountAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

public class AccountCocktailRatingsEntityTypeConfiguration : IEntityTypeConfiguration<AccountCocktailRatings>, IAccountContextEntityConfiguration
{
    public void Configure(EntityTypeBuilder<AccountCocktailRatings> builder)
    {
        builder
            .ToContainer("accounts-account")
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

namespace Cocktails.Api.Infrastructure.EntityConfigurations;

using Cocktails.Api.Domain.Aggregates.AccountAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

public class AccountEntityTypeConfiguration : IEntityTypeConfiguration<Account>, IAccountContextEntityConfiguration
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder
            .ToContainer("accounts-account")
            .HasPartitionKey(x => x.SubjectId)
            .ApplyCamelCasingNamingStrategry()
            .HasKey(x => x.Id);

        builder.Property(x => x.ETag).IsETagConcurrency();

        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.HasDiscriminator(x => x.Discriminator).HasValue("account");

        builder.OwnsOne(
            x => x.PrimaryAddress,
            x =>
            {
                x.ApplyCamelCasingNamingStrategry();
                x.WithOwner();
            });

        builder.OwnsOne(
            x => x.Accessibility,
            x =>
            {
                x.ApplyCamelCasingNamingStrategry();
                x.WithOwner();
            });

        builder.Ignore(x => x.DomainEvents);
    }
}

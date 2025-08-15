namespace Cocktails.Api.Domain.Unit.Tests.Aggregates.AccountAggregate;

using Cocktails.Api.Domain.Aggregates.AccountAggregate;
using Cocktails.Api.Domain.Exceptions;
using FluentAssertions;
using Xunit;

public class AccountNotificationSettingsTests
{
    [Fact]
    public void SetOnNewCocktailAdditionsNotification_ShouldSetNotification()
    {
        // arrange
        var settings = new AccountNotificationSettings(CocktailUpdatedNotification.Always);

        // act
        var updatedSettings = settings.SetOnNewCocktailAdditions(CocktailUpdatedNotification.Never);

        // assert
        updatedSettings.OnNewCocktailAdditions.Should().Be(CocktailUpdatedNotification.Never);
    }

    [Fact]
    public void SetUpdateCocktailNotification_ShouldThrowException_WhenInvalidNotification()
    {
        // arrange
        var settings = new AccountNotificationSettings(CocktailUpdatedNotification.Always);

        // act
        var act = () => settings.SetOnNewCocktailAdditions((CocktailUpdatedNotification)999);

        // assert
        act.Should().Throw<CocktailsApiDomainException>()
            .WithMessage($"{nameof(CocktailUpdatedNotification)} member '999' not found");

        settings.OnNewCocktailAdditions.Should().Be(CocktailUpdatedNotification.Always);
    }
}
namespace Cocktails.Api.Domain.Unit.Tests.Aggregates.AccountAggregate;

using Cocktails.Api.Domain.Aggregates.AccountAggregate;
using Cocktails.Api.Domain.Exceptions;
using FluentAssertions;
using Xunit;

public class AccountNotificationSettingsTests
{
    [Fact]
    public void SetUpdateCocktailNotification_ShouldSetNotification()
    {
        // arrange
        var settings = new AccountNotificationSettings(CocktailUpdateNotification.NewCocktails);

        // act
        var updatedSettings = settings.SetUpdateCocktailNotification(CocktailUpdateNotification.None);

        // assert
        updatedSettings.NewCocktails.Should().Be(CocktailUpdateNotification.None);
    }

    [Fact]
    public void SetUpdateCocktailNotification_ShouldThrowException_WhenInvalidNotification()
    {
        // arrange
        var settings = new AccountNotificationSettings(CocktailUpdateNotification.NewCocktails);

        // act
        var act = () => settings.SetUpdateCocktailNotification((CocktailUpdateNotification)999);

        // assert
        act.Should().Throw<CocktailsApiDomainException>()
            .WithMessage("CocktailUpdateNotification member '999' not found");

        settings.NewCocktails.Should().Be(CocktailUpdateNotification.NewCocktails);
    }
}
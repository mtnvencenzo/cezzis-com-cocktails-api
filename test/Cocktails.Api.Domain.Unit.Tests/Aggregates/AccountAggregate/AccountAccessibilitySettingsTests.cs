namespace Cocktails.Api.Domain.Unit.Tests.Aggregates.AccountAggregate;

using Cocktails.Api.Domain.Aggregates.AccountAggregate;
using FluentAssertions;

public class AccountAccessibilitySettingsTests
{
    [Theory]
    [InlineData(AccessibilityTheme.Light)]
    [InlineData(AccessibilityTheme.Dark)]
    public void SetAccessibilitySettings_ShouldUpdateAccessibility(AccessibilityTheme theme)
    {
        // arrange
        var settings = new AccountAccessibilitySettings(theme: theme);

        // assert
        settings.Theme.Should().Be(theme);
    }
}
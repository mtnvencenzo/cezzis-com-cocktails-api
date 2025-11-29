namespace Cocktails.Api.Domain.Aggregates.AccountAggregate;

using Cocktails.Api.Domain.Common;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class AccountAccessibilitySettings : ValueObject
{
    [JsonInclude]
    public AccessibilityTheme Theme { get; private set; }

    [JsonConstructor]
    protected AccountAccessibilitySettings() { }

    public AccountAccessibilitySettings(AccessibilityTheme theme)
    {
        this.Theme = theme;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Theme;
    }
}

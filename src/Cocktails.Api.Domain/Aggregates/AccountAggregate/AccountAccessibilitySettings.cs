namespace Cocktails.Api.Domain.Aggregates.AccountAggregate;

using Cezzi.Applications;
using Cocktails.Api.Domain.Common;
using Cocktails.Api.Domain.Exceptions;
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

namespace Cocktails.Api.Application.Concerns.Accounts.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

[type: Description("The owned account profile accessibility settings")]
public record AccountAccessibilitySettingsModel
(
    // <example>light</example>
    [property: Required()]
    [property: Description("The accessibility theme")]
    DisplayThemeModel Theme
);
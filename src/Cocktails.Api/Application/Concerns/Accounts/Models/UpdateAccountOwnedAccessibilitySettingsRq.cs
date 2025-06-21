namespace Cocktails.Api.Application.Concerns.Accounts.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

[type: Description("The account owned profile accessibility settings")]
public record UpdateAccountOwnedAccessibilitySettingsRq
(
    // <example>light</example>
    [property: Required()]
    [property: Description("The display theme (light, dark)")]
    DisplayThemeModel Theme
);
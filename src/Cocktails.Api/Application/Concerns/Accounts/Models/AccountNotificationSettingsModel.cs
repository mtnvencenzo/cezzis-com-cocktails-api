namespace Cocktails.Api.Application.Concerns.Accounts.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

[type: Description("The owned account profile notification settings")]
public record AccountNotificationSettingsModel
(
    // <example>always</example>
    [property: Required()]
    [property: Description("The notification setting to use when new cocktail addtions are added")]
    CocktailUpdatedNotificationModel OnNewCocktailAdditions
);
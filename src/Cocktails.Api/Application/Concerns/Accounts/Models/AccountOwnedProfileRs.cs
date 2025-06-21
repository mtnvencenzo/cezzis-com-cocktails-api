namespace Cocktails.Api.Application.Concerns.Accounts.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

/// <summary>The owned account profile response</summary>
[type: Description("The owned account profile response")]
public record AccountOwnedProfileRs
(
    // <example>4493e636-6b1d-4t81-97b9-00d696c1g2f2</example>
    [property: Required()]
    [property: Description("The federated subject identifier for the account")]
    string SubjectId,

    // <example>someone@cezzis.com</example>
    [property: Required()]
    [property: Description("The login email address for the account")]
    string LoginEmail,

    // <example>someone@cezzis.com</example>
    [property: Required()]
    [property: Description("The email address for the account")]
    string Email,

    // <example>John</example>
    [property: Required()]
    [property: Description("The given name on the account")]
    string GivenName,

    // <example>Doe</example>
    [property: Required()]
    [property: Description("The family name on the account")]
    string FamilyName,

    // <example>https://cdn.cezzis.com/account-avatars/b4114bb7-46cf-49ab-b29a-4b20dd69c47e/e878a3b1-ea2a-433c-ba5f-f85df23f03ae.webp</example>
    [property: Required()]
    [property: Description("The avatar image uri for the account")]
    string AvatarUri,

    [property: Description("The optional primary address listed with the account")]
    AccountAddressModel PrimaryAddress,

    // <example>Jamie Johns</example>
    [property: Required()]
    [property: Description("The display name for the account visible to other users")]
    string DisplayName,

    [property: Description("The accessibility settings for the account visible to other users")]
    AccountAccessibilitySettingsModel Accessibility,

    [property: Description("The list of favorite cocktails")]
    List<string> FavoriteCocktails
);
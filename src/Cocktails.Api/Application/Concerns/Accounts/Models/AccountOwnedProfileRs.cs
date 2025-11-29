namespace Cocktails.Api.Application.Concerns.Accounts.Models;

using global::Cocktails.Api.Domain.Aggregates.AccountAggregate;
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

    [property: Description("The accessibility settings for the account")]
    AccountAccessibilitySettingsModel Accessibility,

    [property: Description("The list of favorite cocktails")]
    List<string> FavoriteCocktails,

    [property: Description("The notification settings for the account")]
    AccountNotificationSettingsModel Notifications
)
{
    public static AccountOwnedProfileRs FromAccount(Account account) => new(
        SubjectId: account.SubjectId,
        GivenName: account.GivenName,
        FamilyName: account.FamilyName,
        Email: account.Email,
        LoginEmail: account.LoginEmail ?? account.Email,
        AvatarUri: account.AvatarUri,
        DisplayName: account.DisplayName,
        PrimaryAddress: account.PrimaryAddress != null
            ? new AccountAddressModel(
                AddressLine1: account.PrimaryAddress.AddressLine1 ?? string.Empty,
                AddressLine2: account.PrimaryAddress.AddressLine2 ?? string.Empty,
                City: account.PrimaryAddress.City ?? string.Empty,
                Region: account.PrimaryAddress.Region ?? string.Empty,
                SubRegion: account.PrimaryAddress.SubRegion ?? string.Empty,
                PostalCode: account.PrimaryAddress.PostalCode ?? string.Empty,
                Country: account.PrimaryAddress.Country ?? string.Empty)
            : null,
        Accessibility: account.Accessibility != null
            ? new AccountAccessibilitySettingsModel(Theme: (DisplayThemeModel)account.Accessibility.Theme)
            : new AccountAccessibilitySettingsModel(Theme: DisplayThemeModel.Light),
        FavoriteCocktails: account.FavoriteCocktails ?? [],
        Notifications: account.Notifications != null
            ? new AccountNotificationSettingsModel(
                OnNewCocktailAdditions: (CocktailUpdatedNotificationModel)account.Notifications.OnNewCocktailAdditions)
            : new AccountNotificationSettingsModel(CocktailUpdatedNotificationModel.Always));
};

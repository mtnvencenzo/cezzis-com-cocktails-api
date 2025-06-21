namespace Cocktails.Api.Application.Concerns.Accounts.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

[type: Description("The account owned profile upload request inforamtion")]
public record UpdateAccountOwnedProfileRq
(
    // <example>John</example>
    [property: Required()]
    [property: Description("The given name on the account")]
    string GivenName,

    // <example>Doe</example>
    [property: Required()]
    [property: Description("The family name on the account")]
    string FamilyName,

    // <example>Jamie Johns</example>
    [property: Required()]
    [property: Description("The display for the account visible to other users")]
    string DisplayName,

    [property: Description("The optional primary address listed with the account")]
    AccountAddressModel PrimaryAddress
);
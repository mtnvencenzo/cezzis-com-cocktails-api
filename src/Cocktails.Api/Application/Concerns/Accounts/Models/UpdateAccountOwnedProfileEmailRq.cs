namespace Cocktails.Api.Application.Concerns.Accounts.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

[type: Description("The account owned profile upload email request inforamtion")]
public record UpdateAccountOwnedProfileEmailRq
(
    // <example>tester@cezzis.com</example>
    [property: Required()]
    [property: Description("The email address for the account")]
    string Email
);
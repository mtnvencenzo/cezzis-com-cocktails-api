namespace Cocktails.Api.Application.Concerns.Accounts.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

[type: Description("The account owned profile upload username request information")]
public record ChangeAccountOwnedUsernameRq
(
    // <example>super-fly</example>
    [property: Required()]
    [property: Description("The username to change to for the account")]
    string Username
);
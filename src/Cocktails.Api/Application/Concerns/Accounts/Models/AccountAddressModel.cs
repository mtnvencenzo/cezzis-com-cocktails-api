namespace Cocktails.Api.Application.Concerns.Accounts.Models;

using Microsoft.Identity.Client;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

[type: Description("An owned account profile address")]
public record AccountAddressModel
(
    // <example>123 Anytown Ln.</example>
    [property: Required()]
    [property: Description("The primary street address.")]
    string AddressLine1,

    // <example>Suite #300</example>
    [property: Required()]
    [property: Description("The secondary street address building sub divider")]
    string AddressLine2,

    // <example>Royal Oak</example>
    [property: Required()]
    [property: Description("The city the address is within")]
    string City,

    // <example>MI</example>
    [property: Required()]
    [property: Description("The state or province")]
    string Region,

    // <example>Oakland</example>
    [property: Required()]
    [property: Description("The state or province divered such as county")]
    string SubRegion,

    // <example>48073</example>
    [property: Required()]
    [property: Description("The postal or zip code")]
    string PostalCode,

    // <example>USA</example>
    [property: Required()]
    [property: Description("The country")]
    string Country
);
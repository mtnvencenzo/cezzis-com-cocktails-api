namespace Cocktails.Api.Application.Concerns.Cocktails.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

/// <summary>The cocktail indexnow url response</summary>
public record CocktailIndexNowRs
(
    [property: Required()]
    [property: Description("The cocktail index now url list")]
    IEnumerable<string> UrlList
);

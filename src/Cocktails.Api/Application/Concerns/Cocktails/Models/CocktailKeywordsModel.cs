namespace Cocktails.Api.Application.Concerns.Cocktails.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

/// <summary>The keywords associated with the cocktail recipe</summary>
[type: Description("The keywords associated with the cocktail recipe")]

public record CocktailKeywordsModel
(
    [property: Required()]
    [property: Description("Base spirit keywords")]
    IEnumerable<string> KeywordsBaseSpirit,

    [property: Required()]
    [property: Description("Spirit subtype keywords")]
    IEnumerable<string> KeywordsSpiritSubtype,

    [property: Required()]
    [property: Description("Flavor profile keywords")]
    IEnumerable<string> KeywordsFlavorProfile,

    [property: Required()]
    [property: Description("Cocktail family keywords")]
    IEnumerable<string> KeywordsCocktailFamily,

    [property: Required()]
    [property: Description("Technique keywords")]
    IEnumerable<string> KeywordsTechnique,

    [property: Required()]
    [property: Description("Strength keyword")]
    string KeywordsStrength,

    [property: Required()]
    [property: Description("Temperature keyword")]
    string KeywordsTemperature,

    [property: Required()]
    [property: Description("Season keywords")]
    IEnumerable<string> KeywordsSeason,

    [property: Required()]
    [property: Description("Occasion keywords")]
    IEnumerable<string> KeywordsOccasion,

    [property: Required()]
    [property: Description("Mood keywords")]
    IEnumerable<string> KeywordsMood,

    [property: Required()]
    [property: Description("Search terms keywords")]
    IEnumerable<string> KeywordsSearchTerms
);
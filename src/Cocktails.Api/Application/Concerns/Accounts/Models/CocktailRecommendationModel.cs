namespace Cocktails.Api.Application.Concerns.Accounts.Models;

using global::Cocktails.Api.Application.Behaviors;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

/// <summary>The cocktail recommendation model</summary>
[type: Description("The cocktail recommendation model")]
public record CocktailRecommendationModel
(
    [property: Required()]
    [property: Description("The name of the cocktail or variation")]
    [property: OpenApiExampleDoc<string>("Painkiller (Cezzi's Version)")]
    string Name,

    [property: Required()]
    [property: Description("The ingredients description of the cocktail or variation")]
    [property: OpenApiExampleDoc<string>("3 ounces pimm's no.1")]
    string Ingredients,

    [property: Required()]
    [property: Description("The directions description of the cocktail or variation")]
    [property: OpenApiExampleDoc<string>("Pour everything into a cocktail glass and stir")]
    string Directions
);

/// <summary>
/// 
/// </summary>
public static class CocktailRecommendationModelExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static string GetRecommendation(this CocktailRecommendationModel model) => $"Try the '{model.Name}' cocktail!<br/><br/><b>Ingredients:</b><br/> {model.Ingredients}<br/><br/><b>Directions:</b><br/>{model.Directions}<br/><br/>";
}

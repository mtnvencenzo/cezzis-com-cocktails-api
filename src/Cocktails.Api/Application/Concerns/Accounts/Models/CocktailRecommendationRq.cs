namespace Cocktails.Api.Application.Concerns.Accounts.Models;

using global::Cocktails.Api.Application.Behaviors;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

/// <summary>The cocktail recommendation request</summary>
[type: Description("The cocktail recommendation request")]
public record CocktailRecommendationRq
(
    [property: Required()]
    [property: Description("The cocktail recommendation model")]
    CocktailRecommendationModel Recommendation,

    [property: Required()]
    [property: Description("The google recaptcha verification code returned after being valid")]
    [property: OpenApiExampleDoc<string>("03AFcWeA4HedFMVgSEGAfbZXZdbLu0RXp")]
    string VerificationCode
);
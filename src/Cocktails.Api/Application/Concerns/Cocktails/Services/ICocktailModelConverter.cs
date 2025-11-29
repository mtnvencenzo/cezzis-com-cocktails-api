namespace Cocktails.Api.Application.Concerns.Cocktails.Services;

using global::Cocktails.Api.Application.Concerns.Cocktails.Models;

/// <summary>Converts a Cocktail domain model to a CocktailModel.</summary>
public interface ICocktailModelConverter
{
    CocktailModel ToCocktailModel(Domain.Aggregates.CocktailAggregate.Cocktail cocktail);
}
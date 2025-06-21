namespace Cocktails.Api.Unit.Tests.Application.Commands.Cocktails.CocktailModels;

using FluentAssertions;
using global::Cocktails.Api.Application.Concerns.Accounts.Models;
using Xunit;

public class CocktailRecommendationModelTests
{
    [Fact]
    public void CocktailRecommendationModel_NameProperty_ShouldGetAndSetCorrectly()
    {
        // arrange
        var expectedName = "123";

        // act
        var item = new CocktailRecommendationModel(Name: expectedName, Ingredients: null, Directions: null);
        var actualName = item.Name;

        // assert
        actualName.Should().Be(expectedName);
    }

    [Fact]
    public void CocktailRecommendationModel_IngredientsProperty_ShouldGetAndSetCorrectly()
    {
        // arrange
        var expectedIngredients = "123";

        // act
        var item = new CocktailRecommendationModel(Name: null, Ingredients: expectedIngredients, Directions: null);
        var actualIngredients = item.Ingredients;

        // assert
        actualIngredients.Should().Be(expectedIngredients);
    }

    [Fact]
    public void CocktailRecommendationModel_DirectionsProperty_ShouldGetAndSetCorrectly()
    {
        // arrange
        var expectedDirections = "123";

        // act
        var item = new CocktailRecommendationModel(Name: null, Ingredients: null, Directions: expectedDirections);
        var actualDirections = item.Directions;

        // assert
        actualDirections.Should().Be(expectedDirections);
    }

    [Fact]
    public void CocktailRecommendationModel_Property_Defaults()
    {
        // arrange
        var item = new CocktailRecommendationModel(Name: null, Ingredients: null, Directions: null);

        // assert
        item.Name.Should().BeNull();
        item.Ingredients.Should().BeNull();
        item.Directions.Should().BeNull();
        item.GetRecommendation().Should().Be($"Try the '{item.Name}' cocktail!<br/><br/><b>Ingredients:</b><br/> {item.Ingredients}<br/><br/><b>Directions:</b><br/>{item.Directions}<br/><br/>");
    }

    [Fact]
    public void CocktailRecommendationModel_GetRecommendation_ReturnsCorrecly()
    {
        // arrange
        var item = new CocktailRecommendationModel
        (
            Name: "The Cezzi",
            Ingredients: "1 oz butterfingers, 2 Dashes Parsley",
            Directions: "Stir it up nicely"
        );

        // assert
        item.GetRecommendation().Should().Be($"Try the '{item.Name}' cocktail!<br/><br/><b>Ingredients:</b><br/> {item.Ingredients}<br/><br/><b>Directions:</b><br/>{item.Directions}<br/><br/>");
    }
}

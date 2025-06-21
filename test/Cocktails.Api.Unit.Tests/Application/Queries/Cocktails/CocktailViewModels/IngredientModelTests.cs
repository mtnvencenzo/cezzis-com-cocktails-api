namespace Cocktails.Api.Unit.Tests.Application.Queries.Cocktails.CocktailViewModels;

using FluentAssertions;
using global::Cocktails.Api.Application.Concerns.Cocktails;
using global::Cocktails.Api.Application.Concerns.Cocktails.Models;
using Xunit;

public class IngredientModelTests
{
    [Fact]
    public void ingredient_model_properties_are_set_correctly()
    {
        // act
        var ingredient = new IngredientModel
        (
            Name: "Sugar",
            Units: 2.5f,
            UoM: UofMTypeModel.Teaspoon,
            Types: [IngredientTypeModel.Syrup],
            Applications: [IngredientApplicationModel.Base],
            Preparation: PreparationTypeModel.None,
            Suggestions: "Add to taste",
            Requirement: IngredientRequirementTypeModel.Optional,
            Display: "DisplayName"
        );

        // assert
        ingredient.Name.Should().Be("Sugar");
        ingredient.Units.Should().Be(2.5f);
        ingredient.UoM.Should().Be(UofMTypeModel.Teaspoon);
        ingredient.Types.Should().BeEquivalentTo([IngredientTypeModel.Syrup]);
        ingredient.Preparation.Should().Be(PreparationTypeModel.None);
        ingredient.Suggestions.Should().Be("Add to taste");
        ingredient.Requirement.Should().Be(IngredientRequirementTypeModel.Optional);
    }
}

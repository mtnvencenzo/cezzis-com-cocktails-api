namespace Cocktails.Api.Unit.Tests.Application.Queries.Cocktails.CocktailViewModels;

using AutoBogus;
using FluentAssertions;
using global::Cocktails.Api.Application.Concerns.Cocktails;
using global::Cocktails.Api.Application.Concerns.Cocktails.Models;
using Xunit;

public class CocktailModelTests
{
    [Fact]
    public void CocktailModel_IdProperty_ShouldGetAndSetCorrectly()
    {
        // arrange
        var expectedId = "123";

        // act
        var cocktailModel = new AutoFaker<CocktailModel>()
            .RuleFor(x => x.Id, expectedId)
            .Generate();

        // Assert
        var actualId = cocktailModel.Id;
        actualId.Should().Be(expectedId);
    }

    [Fact]
    public void CocktailModel_TitleProperty_ShouldGetAndSetCorrectly()
    {
        // arrange
        var expectedTitle = "Mojito";

        // act
        var cocktailModel = new AutoFaker<CocktailModel>()
            .RuleFor(x => x.Title, expectedTitle)
            .Generate();

        // assert
        var actualTitle = cocktailModel.Title;
        actualTitle.Should().Be(expectedTitle);
    }

    [Fact]
    public void CocktailModel_DescriptionProperty_ShouldGetAndSetCorrectly()
    {
        // arrange
        var expectedContent = "Refreshing cocktail";

        // act
        var cocktailModel = new AutoFaker<CocktailModel>()
            .RuleFor(x => x.Content, expectedContent)
            .Generate();

        // assert
        cocktailModel.Content.Should().Be(expectedContent);
    }

    [Fact]
    public void CocktailModel_SearchableTitlesProperty_ShouldGetAndSetCorrectly()
    {
        // arrange
        var expectedSearchableTitles = new List<string> { "Mojito", "Cocktail" };

        // act
        var cocktailModel = new AutoFaker<CocktailModel>()
            .RuleFor(x => x.SearchableTitles, expectedSearchableTitles)
            .Generate();

        // assert
        cocktailModel.SearchableTitles.Should().BeEquivalentTo(expectedSearchableTitles);
    }

    [Fact]
    public void CocktailModel_TagsProperty_ShouldGetAndSetCorrectly()
    {
        // arrange
        var expectedTags = new List<string> { "refreshing", "summer" };

        // act
        var cocktailModel = new AutoFaker<CocktailModel>()
            .RuleFor(x => x.Tags, expectedTags)
            .Generate();

        // assert
        cocktailModel.Tags.Should().BeEquivalentTo(expectedTags);
    }

    [Fact]
    public void CocktailModel_IngredientsProperty_ShouldGetAndSetCorrectly()
    {
        // arrange
        var expectedIngredients = new List<IngredientModel>
        {
            AutoFaker.Generate<IngredientModel>(),
            AutoFaker.Generate<IngredientModel>()
        };

        // act
        var cocktailModel = new AutoFaker<CocktailModel>()
            .RuleFor(x => x.Ingredients, expectedIngredients)
            .Generate();

        // assert
        cocktailModel.Ingredients.Should().BeEquivalentTo(expectedIngredients);
    }

    [Fact]
    public void CocktailModel_ServesProperty_ShouldGetAndSetCorrectly()
    {
        // arrange
        var expectedServes = 2;

        // act
        var cocktailModel = new AutoFaker<CocktailModel>()
            .RuleFor(x => x.Serves, expectedServes)
            .Generate();

        // assert
        cocktailModel.Serves.Should().Be(expectedServes);
    }

    [Fact]
    public void CocktailModel_IsIba_ShouldGetAndSetCorrectly()
    {
        // arrange
        var expected = true;

        // act
        var cocktailModel = new AutoFaker<CocktailModel>()
            .RuleFor(x => x.IsIba, expected)
            .Generate();

        // assert
        cocktailModel.IsIba.Should().Be(expected);
    }

    [Fact]
    public void CocktailModel_GlasswareProperty_ShouldGetAndSetCorrectly()
    {
        // arrange
        var expectedGlassware = new List<GlasswareTypeModel> { GlasswareTypeModel.Highball, GlasswareTypeModel.CocktailGlass };

        // act
        var cocktailModel = new AutoFaker<CocktailModel>()
            .RuleFor(x => x.Glassware, expectedGlassware)
            .Generate();

        // assert
        cocktailModel.Glassware.Should().BeEquivalentTo(expectedGlassware);
    }

    [Fact]
    public void CocktailModel_DescriptiveTitleProperty_ShouldGetAndSetCorrectly()
    {
        // arrange
        var expectedDescriptiveTitle = "Classic Mojito";

        // act
        var cocktailModel = new AutoFaker<CocktailModel>()
            .RuleFor(x => x.DescriptiveTitle, expectedDescriptiveTitle)
            .Generate();

        // assert
        cocktailModel.DescriptiveTitle.Should().BeEquivalentTo(expectedDescriptiveTitle);
    }
}
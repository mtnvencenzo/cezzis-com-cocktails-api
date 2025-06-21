namespace Cocktails.Api.Application.Utilities;

using Cocktails.Api.Application.Concerns.Cocktails.Models;
using Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using Cocktails.Api.Domain.Config;
using FluentAssertions;
using Microsoft.Extensions.Options;

public static class AssertionHelpers
{
    public static void AssertBeesKneesCocktail(IServiceProvider sp, CocktailModel cocktail)
    {
        var options = sp.GetRequiredService<IOptions<CocktailsApiConfig>>().Value;

        cocktail.Should().NotBeNull();
        cocktail.Id.Should().Be("bees-knees");
        cocktail.Title.Should().Be("Bee's Knees");
        cocktail.Content.Should().NotBeNullOrWhiteSpace();
        cocktail.Content.Should().Contain($"{options.BaseImageUri}/traditional-bees-knees-cocktail-main.webp");
        cocktail.Tags.Should().ContainSingle();
        cocktail.Tags.First().Should().Be("Traditional");
        cocktail.Serves.Should().Be(1);
        cocktail.Glassware.Should().HaveCount(2);
        cocktail.Glassware.First().Should().Be(GlasswareTypeModel.Coupe);
        cocktail.Glassware.Last().Should().Be(GlasswareTypeModel.CocktailGlass);
        cocktail.DescriptiveTitle.Should().Be("The Bee's Knees Cocktail: A Roaring Twenties Delight");
        cocktail.IsIba.Should().BeTrue();
        cocktail.Ingredients.Should().HaveCount(4);

        var ingredients = cocktail.Ingredients.ToList();

        ingredients[0].Name.Should().Be("Gin");
        ingredients[0].Units.Should().Be(1.5F);
        ingredients[0].UoM.Should().Be(UofMTypeModel.Ounces);
        ingredients[0].Types.ToList().Should().BeEquivalentTo([IngredientTypeModel.Spirit]);
        ingredients[0].Applications.ToList().Should().BeEquivalentTo([IngredientApplicationModel.Base]);
        ingredients[0].Suggestions.Should().BeEmpty();
        ingredients[0].Requirement.Should().Be(IngredientRequirementTypeModel.Required);
        ingredients[0].Preparation.Should().Be(PreparationTypeModel.None);

        ingredients[1].Name.Should().Be("Lemon Juice");
        ingredients[1].Units.Should().Be(0.75F);
        ingredients[1].UoM.Should().Be(UofMTypeModel.Ounces);
        ingredients[1].Types.Should().BeEquivalentTo([IngredientTypeModel.Juice]);
        ingredients[1].Applications.ToList().Should().BeEquivalentTo([IngredientApplicationModel.Additional]);
        ingredients[1].Suggestions.Should().BeEmpty();
        ingredients[1].Requirement.Should().Be(IngredientRequirementTypeModel.Required);
        ingredients[1].Preparation.Should().Be(PreparationTypeModel.FreshlySqueezed);

        ingredients[2].Name.Should().Be("Honey Syrup");
        ingredients[2].Units.Should().Be(0.75F);
        ingredients[2].UoM.Should().Be(UofMTypeModel.Ounces);
        ingredients[2].Types.Should().BeEquivalentTo([IngredientTypeModel.Syrup]);
        ingredients[2].Applications.ToList().Should().BeEquivalentTo([IngredientApplicationModel.Additional]);
        ingredients[2].Suggestions.Should().BeEmpty();
        ingredients[2].Requirement.Should().Be(IngredientRequirementTypeModel.Required);
        ingredients[2].Preparation.Should().Be(PreparationTypeModel.None);

        ingredients[3].Name.Should().Be("Lemon Wheel");
        ingredients[3].Units.Should().Be(1.0F);
        ingredients[3].UoM.Should().Be(UofMTypeModel.Item);
        ingredients[3].Types.Should().BeEquivalentTo([IngredientTypeModel.Fruit]);
        ingredients[3].Applications.ToList().Should().BeEquivalentTo([IngredientApplicationModel.Garnishment]);
        ingredients[3].Suggestions.Should().BeEmpty();
        ingredients[3].Requirement.Should().Be(IngredientRequirementTypeModel.Required);
        ingredients[3].Preparation.Should().Be(PreparationTypeModel.None);
    }

    public static void AssertCocktailListItem(Cocktail expected, CocktailsListModel actual, CocktailDataIncludeModel[] includes)
    {
        actual.Should().NotBeNull();
        actual.Id.Should().Be(expected.Id);
        actual.Title.Should().Be(expected.Title);
        actual.IsIba.Should().Be(expected.IsIba);

        if (includes.Contains(CocktailDataIncludeModel.descriptiveTitle))
        {
            actual.DescriptiveTitle.Should().Be(expected.DescriptiveTitle);
        }
        else
        {
            actual.DescriptiveTitle.Should().BeNull();
        }

        if (includes.Contains(CocktailDataIncludeModel.mainImages))
        {
            actual.MainImages.Should().HaveSameCount(expected.Images.Where(x => x.Type == CocktailImageType.Main));
        }
        else
        {
            actual.MainImages.Should().BeNull();
        }

        if (includes.Contains(CocktailDataIncludeModel.searchTiles))
        {
            actual.SearchTiles.Should().HaveSameCount(expected.Images.Where(x => x.Type == CocktailImageType.SearchTile));
        }
        else
        {
            actual.SearchTiles.Should().BeNull();
        }
    }

    public static void AssertCocktailModelMatches(Cocktail expected, CocktailModel actual, CocktailsApiConfig config)
    {
        expected.Should().NotBeNull();
        actual.Should().NotBeNull();

        actual.Id.Should().Be(expected.Id);
        actual.Title.Should().Be(expected.Title);
        actual.DescriptiveTitle.Should().Be(expected.DescriptiveTitle);
        actual.Ingredients.Should().HaveCount(expected.Ingredients.Count);
        actual.IsIba.Should().Be(expected.IsIba);

        foreach (var img in actual.MainImages)
        {
            img.Uri.Should().StartWith($"{config.BaseImageUri}/");
        }

        foreach (var img in actual.SearchTiles)
        {
            img.Uri.Should().StartWith($"{config.BaseImageUri}/");
        }
    }
}

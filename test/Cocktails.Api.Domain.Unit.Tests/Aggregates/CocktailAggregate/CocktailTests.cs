namespace Cocktails.Api.Domain.Unit.Tests.Aggregates.CocktailAggregate;

using Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using Cocktails.Api.Domain.Aggregates.IngredientAggregate;
using Cocktails.Api.Infrastructure;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

public class CocktailTests
{
    [Fact]
    public void GetIngredientsMarkDownDescription_ShouldReturnCorrectDescription()
    {
        var ing1 = this.CreateIngredient("id-1", "Rum", "Rum", [IngredientType.Spirit.ToString()], [IngredientApplication.Base.ToString()]);
        var ing2 = this.CreateIngredient("id-2", "Lime Juice", "Lime Juice", [IngredientType.Fruit.ToString()], [IngredientApplication.Base.ToString()]);
        var ing3 = this.CreateIngredient("id-3", "Simple Syrup", "Simple Syrup", [IngredientType.Syrup.ToString()], [IngredientApplication.Base.ToString()]);
        var ing4 = this.CreateIngredient("id-4", "Mint Leaves", "Mint Leaves", [IngredientType.Herb.ToString()], [IngredientApplication.Muddle.ToString()]);
        var ing5 = this.CreateIngredient("id-5", "Club Soda", "Club Soda", [IngredientType.Dilution.ToString()], [IngredientApplication.Additional.ToString()]);
        var ing6 = this.CreateIngredient("id-6", "Lime Wedge", "Lime Wedge", [IngredientType.Fruit.ToString()], [IngredientApplication.Garnishment.ToString(), IngredientApplication.Base.ToString()]);

        // Arrange
        var cocktailItem = new Cocktail("1", "Mojito")
            .SetIngredient(ing1, 2, UofM.Ounces)
            .SetIngredient(ing2, 1, UofM.Ounces)
            .SetIngredient(ing3, .75F, UofM.Ounces)
            .SetIngredient(ing4, 8, UofM.Item)
            .SetIngredient(ing5, 1, UofM.Splash)
            .SetIngredient(ing6, 1, UofM.Discretion);

        var expected = $"- **2 ounces** rum{Environment.NewLine}- **1 ounce** lime juice{Environment.NewLine}- **3/4 ounce** simple syrup{Environment.NewLine}- **8 Mint Leaves**{Environment.NewLine}- **Splash** of club soda{Environment.NewLine}- **Garnish** with a lime wedge";

        // Act
        var description = cocktailItem.GetIngredientsMarkDownDescription();

        // Assert
        description.Should().Be(expected);
    }

    [Fact]
    public void Id_ShouldReturnCorrectValue()
    {
        // Arrange
        var id = "1";
        var cocktailItem = new Cocktail(id, "Mojito");

        // Act
        var result = cocktailItem.Id;

        // Assert
        result.Should().Be(id);
    }

    [Fact]
    public void Title_ShouldReturnCorrectValue()
    {
        // Arrange
        var title = "Mojito";
        var cocktailItem = new Cocktail("1", title);

        // Act
        var result = cocktailItem.Title;

        // Assert
        result.Should().Be(title);
    }

    [Fact]
    public void Description_ShouldReturnCorrectValue()
    {
        // Arrange
        var description = "Refreshing cocktail";
        var cocktailItem = new Cocktail("1", "Mojito", description: description);

        // Act
        var result = cocktailItem.Description;

        // Assert
        result.Should().Be(description);
    }

    [Fact]
    public void Content_ShouldReturnCorrectValue()
    {
        // arrange
        var content = "Refreshing cocktail";
        var cocktailItem = new Cocktail("1", "Mojito");
        cocktailItem.SetContent(content);

        // act
        var result = cocktailItem.Content;

        // assert
        result.Should().Be(content);
    }

    [Fact]
    public void DescriptiveTitle_ShouldSetAndGetCorrectValue()
    {
        // Arrange
        var descriptiveTitle = "Delicious Mojito";
        var cocktailItem = new Cocktail("1", "Mojito", descriptiveTitle: descriptiveTitle);
        var result = cocktailItem.DescriptiveTitle;

        // Assert
        result.Should().Be(descriptiveTitle);
    }

    [Fact]
    public void SearchableTitles_ShouldSetAndGetCorrectValue()
    {
        // Arrange
        string[] searchableTitles = ["Mojito", "Cocktail"];

        var cocktailItem = new Cocktail("1", "Mojito")
            .SetSearchableTitles(searchableTitles);

        var result = cocktailItem.SearchableTitles;

        // Assert
        result.Should().BeEquivalentTo(searchableTitles);
    }

    [Fact]
    public void Eras_ShouldSetAndGetCorrectValue()
    {
        // Arrange
        string[] eras = ["refreshing", "summer"];

        var cocktailItem = new Cocktail("1", "Mojito")
            .SetEras(eras);

        var result = cocktailItem.Eras;

        // Assert
        result.Should().BeEquivalentTo(eras);
    }

    [Fact]
    public void IsIba_ShouldSetAndGetCorrectValue()
    {
        // Arrange
        var expected = true;
        var cocktailItem = new Cocktail("1", "Mojito").SetIsIba(expected);
        var result = cocktailItem.IsIba;

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Serves_ShouldSetAndGetCorrectValue()
    {
        // Arrange
        var serves = 2;
        var cocktailItem = new Cocktail("1", "Mojito")
            .SetServes(serves);

        var result = cocktailItem.Serves;

        // Assert
        result.Should().Be(serves);
    }

    [Fact]
    public void Glassware_ShouldSetAndGetCorrectValue()
    {
        // Arrange
        GlasswareType[] glassware = [GlasswareType.Collins, GlasswareType.Highball];

        var cocktailItem = new Cocktail("1", "Mojito")
            .SetGlassware(glassware);

        var result = cocktailItem.Glassware;

        // Assert
        result.Should().BeEquivalentTo(glassware.Select(x => x.ToString()));
    }

    private Ingredient CreateIngredient(
    string id,
    string name,
    string shelfDisplay,
    List<string> types,
    List<string> applications) => new(id, name, shelfDisplay, types, applications, DateTimeOffset.Now, DateTimeOffset.Now);

    private Ingredient GetIngredient(string id)
    {
        var store = new IngredientsDataStore();

        return store.Ingredients.First(x => x.Id == id);
    }
}

namespace Cocktails.Api.Domain.Unit.Tests.Aggregates.CocktailAggregate;

using Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using Cocktails.Api.Domain.Aggregates.IngredientAggregate;
using Cocktails.Api.Infrastructure;
using FluentAssertions;
using Xunit;

public class IngredientTests
{
    [Fact]
    public void ingredient___properties_are_set_correctly()
    {
        // Arrange
        var name = "Sugar";
        var units = 2.5f;
        var unitOfMeasure = UofM.Cups;
        var types = new List<string>([IngredientType.Herb.ToString()]);
        var applications = new List<string>([IngredientApplication.Base.ToString()]);
        var prep = PreparationType.None;
        var suggestions = "Add to taste";
        var requirement = IngredientRequirement.Required;

        // Act
        var baseIngredient = this.CreateIngredient(
            "sugar",
            name,
            "Sugar",
            types,
            applications);

        var ingredient = new CocktailIngredient(baseIngredient, units, unitOfMeasure, prep, suggestions, requirement);

        // Assert
        ingredient.Name.Should().Be(name);
        ingredient.Units.Should().Be(units);
        ingredient.UoM.Should().Be(unitOfMeasure);
        ingredient.Types.Should().BeEquivalentTo(types);
        ingredient.Preparation.Should().Be(prep);
        ingredient.Suggestions.Should().Be(suggestions);
        ingredient.Requirement.Should().Be(requirement);
    }

    [Fact]
    public void getIngredientSuffix___returns_correct_ingredient_suffix_for_to_taste()
    {
        // Arrange
        var baseIngredient = this.GetIngredient("salt");
        var ingredient = new CocktailIngredient(baseIngredient, 0.5f, UofM.ToTaste);

        // Act
        var ingredientSuffix = ingredient.GetIngredientSuffix();

        // Assert
        ingredientSuffix.Should().Be("to taste");
    }

    [Fact]
    public void getIngredientSuffix___returns_correct_ingredient_suffix_for_garnishment()
    {
        // Arrange
        var baseIngredient = this.GetIngredient("limes");
        var ingredient = new CocktailIngredient(baseIngredient, "lime-wheel", 1.0f, UofM.Item);

        // Act
        var ingredientSuffix = ingredient.GetIngredientSuffix();

        // Assert
        ingredientSuffix.Should().Be("for garnishment");
    }

    [Fact]
    public void getIngredientSuffix___returns_correct_ingredient_suffix_for_topoff()
    {
        // Arrange
        var baseIngredient = this.GetIngredient("soda-water");
        var ingredient = new CocktailIngredient(baseIngredient, 0.25f, UofM.Topoff);

        // Act
        var ingredientSuffix = ingredient.GetIngredientSuffix();

        // Assert
        ingredientSuffix.Should().Be("(top off)");
    }

    [Fact]
    public void getIngredientSuffix___returns_correct_ingredient_suffix_for_muddle_with_suggestions()
    {
        // Arrange
        var baseIngredient = this.GetIngredient("limes");
        var ingredient = new CocktailIngredient(baseIngredient, "lime", 6.0f, UofM.Item, suggestions: "lightly muddled");

        // Act
        var ingredientSuffix = ingredient.GetIngredientSuffix();

        // Assert
        ingredientSuffix.Should().Be("(lightly muddled)");
    }

    [Fact]
    public void getIngredientSuffix___returns_correct_ingredient_suffix_for_muddle_without_suggestions()
    {
        // Arrange
        var baseIngredient = this.GetIngredient("mint");
        var ingredient = new CocktailIngredient(baseIngredient, "mint-leaves", 6.0f, UofM.Item);

        // Act
        var ingredientSuffix = ingredient.GetIngredientSuffix();

        // Assert
        ingredientSuffix.Should().BeEmpty();
    }

    [Fact]
    public void GetMeasurementDisplay_Should_Return_Correct_Display()
    {
        // Arrange
        var baseIngredient = this.GetIngredient("sugar");
        var ingredient = new CocktailIngredient(baseIngredient, 0.5F, UofM.Cups);

        // Act
        var measurementDisplay = ingredient.GetMeasurementDisplay();

        // Assert
        measurementDisplay.Should().Be("1/2 cup");
    }

    [Fact]
    public void GetMeasurementDisplay_Should_Return_Empty_String_When_UoM_Is_Discretion()
    {
        // Arrange
        var baseIngredient = this.GetIngredient("salt");
        var ingredient = new CocktailIngredient(baseIngredient, 1.0F, UofM.Discretion);

        // Act
        var measurementDisplay = ingredient.GetMeasurementDisplay();

        // Assert
        measurementDisplay.Should().Be(string.Empty);
    }

    [Fact]
    public void GetMeasurementDisplay_Should_Return_Correct_Display_When_Condensed_Is_False()
    {
        // Arrange
        var baseIngredient = this.GetIngredient("agave-nectar");
        var ingredient = new CocktailIngredient(baseIngredient, 1.5F, UofM.Teaspoon);

        // Act
        var measurementDisplay = ingredient.GetMeasurementDisplay(false);

        // Assert
        measurementDisplay.Should().Be("1 1/2 teaspoons");
    }

    [Theory]
    [InlineData(UofM.Ounces, 0.125f, "1/8 ounce")]
    [InlineData(UofM.Ounces, 0.20f, "1/5 ounce")]
    [InlineData(UofM.Ounces, 0.25f, "1/4 ounce")]
    [InlineData(UofM.Ounces, 0.33f, "1/3 ounce")]
    [InlineData(UofM.Ounces, 0.5f, "1/2 ounce")]
    [InlineData(UofM.Ounces, 0.75f, "3/4 ounce")]
    [InlineData(UofM.Ounces, 1.0f, "1 ounce")]
    [InlineData(UofM.Ounces, 2.5f, "2 1/2 ounces")]
    [InlineData(UofM.Cups, 0.125f, "1/8 cup")]
    [InlineData(UofM.Cups, 0.20f, "1/5 cup")]
    [InlineData(UofM.Cups, 0.25f, "1/4 cup")]
    [InlineData(UofM.Cups, 0.33f, "1/3 cup")]
    [InlineData(UofM.Cups, 0.5f, "1/2 cup")]
    [InlineData(UofM.Cups, 0.75f, "3/4 cup")]
    [InlineData(UofM.Cups, 1.0f, "1 cup")]
    [InlineData(UofM.Cups, 2.5f, "2 1/2 cups")]
    [InlineData(UofM.Dashes, 0.125f, "1/8 dash")]
    [InlineData(UofM.Dashes, 0.20f, "1/5 dash")]
    [InlineData(UofM.Dashes, 0.25f, "1/4 dash")]
    [InlineData(UofM.Dashes, 0.33f, "1/3 dash")]
    [InlineData(UofM.Dashes, 0.5f, "1/2 dash")]
    [InlineData(UofM.Dashes, 0.75f, "3/4 dash")]
    [InlineData(UofM.Dashes, 1.0f, "1 dash")]
    [InlineData(UofM.Dashes, 2.5f, "2 1/2 dashes")]
    [InlineData(UofM.Tablespoon, 0.125f, "1/8 tablespoon")]
    [InlineData(UofM.Tablespoon, 0.20f, "1/5 tablespoon")]
    [InlineData(UofM.Tablespoon, 0.25f, "1/4 tablespoon")]
    [InlineData(UofM.Tablespoon, 0.33f, "1/3 tablespoon")]
    [InlineData(UofM.Tablespoon, 0.5f, "1/2 tablespoon")]
    [InlineData(UofM.Tablespoon, 0.75f, "3/4 tablespoon")]
    [InlineData(UofM.Tablespoon, 1.0f, "1 tablespoon")]
    [InlineData(UofM.Tablespoon, 2.5f, "2 1/2 tablespoons")]
    [InlineData(UofM.Teaspoon, 0.125f, "1/8 teaspoon")]
    [InlineData(UofM.Teaspoon, 0.20f, "1/5 teaspoon")]
    [InlineData(UofM.Teaspoon, 0.25f, "1/4 teaspoon")]
    [InlineData(UofM.Teaspoon, 0.33f, "1/3 teaspoon")]
    [InlineData(UofM.Teaspoon, 0.5f, "1/2 teaspoon")]
    [InlineData(UofM.Teaspoon, 0.75f, "3/4 teaspoon")]
    [InlineData(UofM.Teaspoon, 1.0f, "1 teaspoon")]
    [InlineData(UofM.Teaspoon, 2.5f, "2 1/2 teaspoons")]
    [InlineData(UofM.Barspoon, 0.125f, "1/8 barspoon")]
    [InlineData(UofM.Barspoon, 0.20f, "1/5 barspoon")]
    [InlineData(UofM.Barspoon, 0.25f, "1/4 barspoon")]
    [InlineData(UofM.Barspoon, 0.33f, "1/3 barspoon")]
    [InlineData(UofM.Barspoon, 0.5f, "1/2 barspoon")]
    [InlineData(UofM.Barspoon, 0.75f, "3/4 barspoon")]
    [InlineData(UofM.Barspoon, 1.0f, "1 barspoon")]
    [InlineData(UofM.Barspoon, 2.5f, "2 1/2 barspoons")]
    [InlineData(UofM.Item, 0.125f, "1/8")]
    [InlineData(UofM.Item, 0.20f, "1/5")]
    [InlineData(UofM.Item, 0.25f, "1/4")]
    [InlineData(UofM.Item, 0.33f, "1/3")]
    [InlineData(UofM.Item, 0.5f, "1/2")]
    [InlineData(UofM.Item, 0.75f, "3/4")]
    [InlineData(UofM.Item, 1.0f, "1")]
    [InlineData(UofM.Item, 2.5f, "2 1/2")]
    [InlineData(UofM.ToTaste, 0.5f, "")]
    [InlineData(UofM.Topoff, 0.5f, "")]
    public void GetMeasurementDisplay_Should_Return_Correct_Display_For_unit_of_measure(UofM unitOfMeasure, float units, string expectedDisplay)
    {
        // Arrange
        var baseIngredient = this.GetIngredient("mint");
        var ingredient = new CocktailIngredient(baseIngredient, "mint-leaves", units, unitOfMeasure);

        // Act
        var measurementDisplay = ingredient.GetMeasurementDisplay(condensed: false);

        // Assert
        measurementDisplay.Should().Be(expectedDisplay);
    }

    [Theory]
    [InlineData(UofM.Ounces, 0.125f, "1/8 oz")]
    [InlineData(UofM.Ounces, 0.20f, "1/5 oz")]
    [InlineData(UofM.Ounces, 0.25f, "1/4 oz")]
    [InlineData(UofM.Ounces, 0.33f, "1/3 oz")]
    [InlineData(UofM.Ounces, 0.5f, "1/2 oz")]
    [InlineData(UofM.Ounces, 0.75f, "3/4 oz")]
    [InlineData(UofM.Ounces, 1.0f, "1 oz")]
    [InlineData(UofM.Ounces, 2.5f, "2 1/2 oz")]
    [InlineData(UofM.Cups, 0.125f, "1/8 cup")]
    [InlineData(UofM.Cups, 0.20f, "1/5 cup")]
    [InlineData(UofM.Cups, 0.25f, "1/4 cup")]
    [InlineData(UofM.Cups, 0.33f, "1/3 cup")]
    [InlineData(UofM.Cups, 0.5f, "1/2 cup")]
    [InlineData(UofM.Cups, 0.75f, "3/4 cup")]
    [InlineData(UofM.Cups, 1.0f, "1 cup")]
    [InlineData(UofM.Cups, 2.5f, "2 1/2 cups")]
    [InlineData(UofM.Dashes, 0.125f, "1/8 dash")]
    [InlineData(UofM.Dashes, 0.20f, "1/5 dash")]
    [InlineData(UofM.Dashes, 0.25f, "1/4 dash")]
    [InlineData(UofM.Dashes, 0.33f, "1/3 dash")]
    [InlineData(UofM.Dashes, 0.5f, "1/2 dash")]
    [InlineData(UofM.Dashes, 0.75f, "3/4 dash")]
    [InlineData(UofM.Dashes, 1.0f, "1 dash")]
    [InlineData(UofM.Dashes, 2.5f, "2 1/2 dashes")]
    [InlineData(UofM.Tablespoon, 0.125f, "1/8 tbsp")]
    [InlineData(UofM.Tablespoon, 0.20f, "1/5 tbsp")]
    [InlineData(UofM.Tablespoon, 0.25f, "1/4 tbsp")]
    [InlineData(UofM.Tablespoon, 0.33f, "1/3 tbsp")]
    [InlineData(UofM.Tablespoon, 0.5f, "1/2 tbsp")]
    [InlineData(UofM.Tablespoon, 0.75f, "3/4 tbsp")]
    [InlineData(UofM.Tablespoon, 1.0f, "1 tbsp")]
    [InlineData(UofM.Tablespoon, 2.5f, "2 1/2 tbsp")]
    [InlineData(UofM.Teaspoon, 0.125f, "1/8 tsp")]
    [InlineData(UofM.Teaspoon, 0.20f, "1/5 tsp")]
    [InlineData(UofM.Teaspoon, 0.25f, "1/4 tsp")]
    [InlineData(UofM.Teaspoon, 0.33f, "1/3 tsp")]
    [InlineData(UofM.Teaspoon, 0.5f, "1/2 tsp")]
    [InlineData(UofM.Teaspoon, 0.75f, "3/4 tsp")]
    [InlineData(UofM.Teaspoon, 1.0f, "1 tsp")]
    [InlineData(UofM.Teaspoon, 2.5f, "2 1/2 tsp")]
    [InlineData(UofM.Barspoon, 0.125f, "1/8 barspoon")]
    [InlineData(UofM.Barspoon, 0.20f, "1/5 barspoon")]
    [InlineData(UofM.Barspoon, 0.25f, "1/4 barspoon")]
    [InlineData(UofM.Barspoon, 0.33f, "1/3 barspoon")]
    [InlineData(UofM.Barspoon, 0.5f, "1/2 barspoon")]
    [InlineData(UofM.Barspoon, 0.75f, "3/4 barspoon")]
    [InlineData(UofM.Barspoon, 1.0f, "1 barspoon")]
    [InlineData(UofM.Barspoon, 2.5f, "2 1/2 barspoons")]
    [InlineData(UofM.Item, 0.125f, "1/8")]
    [InlineData(UofM.Item, 0.20f, "1/5")]
    [InlineData(UofM.Item, 0.25f, "1/4")]
    [InlineData(UofM.Item, 0.33f, "1/3")]
    [InlineData(UofM.Item, 0.5f, "1/2")]
    [InlineData(UofM.Item, 0.75f, "3/4")]
    [InlineData(UofM.Item, 1.0f, "1")]
    [InlineData(UofM.Item, 2.5f, "2 1/2")]
    [InlineData(UofM.ToTaste, 0.5f, "")]
    [InlineData(UofM.Topoff, 0.5f, "")]
    public void GetMeasurementDisplay_Should_Return_Correct_Display_For_unit_of_measure_condensed(UofM unitOfMeasure, float units, string expectedDisplay)
    {
        // Arrange
        var baseIngredient = this.GetIngredient("mint");
        var ingredient = new CocktailIngredient(baseIngredient, "mint-leaves", units, unitOfMeasure);

        // Act
        var measurementDisplay = ingredient.GetMeasurementDisplay(condensed: true);

        // Assert
        measurementDisplay.Should().Be(expectedDisplay);
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

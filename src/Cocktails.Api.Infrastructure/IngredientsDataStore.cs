namespace Cocktails.Api.Infrastructure;

using Cocktails.Api.Domain.Aggregates.IngredientAggregate;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public class IngredientsDataStore
{
    private readonly static List<Ingredient> ingredients = [];
    private readonly static Lock sync = new();
    private readonly static JsonSerializerOptions jsonSerializerOptions;

    static IngredientsDataStore()
    {
        jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            AllowTrailingCommas = true
        };

        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    }

    public ReadOnlyCollection<Ingredient> Ingredients => this.GetIngredients().AsReadOnly();

    private List<Ingredient> GetIngredients()
    {
        if (ingredients.Count > 0)
        {
            return ingredients;
        }

        using (sync.EnterScope())
        {
            if (ingredients.Count == 0)
            {
                var json = Encoding.UTF8.GetString(Resources.Ingredients.Ingredients._ingredients_data);

                // set traditional cocktails
                ingredients.AddRange(JsonSerializer.Deserialize<List<Ingredient>>(json, jsonSerializerOptions));
            }
        }

        return ingredients;
    }
}

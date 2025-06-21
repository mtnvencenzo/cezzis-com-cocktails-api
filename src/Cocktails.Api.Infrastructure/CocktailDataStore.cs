namespace Cocktails.Api.Infrastructure;

using Cezzi.Applications.Extensions;
using Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using Cocktails.Api.Domain.Aggregates.IngredientAggregate;
using Cocktails.Api.Domain.Common;
using Cocktails.Api.Domain.Config;
using Cocktails.Api.Infrastructure.Resources;
using Cocktails.Api.Infrastructure.Resources.ModernCocktails;
using Cocktails.Api.Infrastructure.Resources.TraditionalCocktails;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class CocktailDataStore(IOptions<CocktailsApiConfig> cocktailsApiConfig, IngredientsDataStore ingredientDataStore)
{
    private readonly static string bomMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
    private readonly static List<Cocktail> cocktails = [];
    private readonly static Lock sync = new();
    private readonly static JsonSerializerOptions jsonSerializerOptions;

    static CocktailDataStore()
    {
        jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            AllowTrailingCommas = true
        };

        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    }

    public ReadOnlyCollection<Cocktail> Cocktails => this.GetCocktails().AsReadOnly();

    private List<Cocktail> GetCocktails()
    {
        if (cocktails.Count > 0)
        {
            return cocktails;
        }

        using (sync.EnterScope())
        {
            if (cocktails.Count == 0)
            {
                var ingredients = ingredientDataStore.Ingredients.ToList();

                List<Cocktail> availableCocktails = [
                    .. this.GetCocktails(TraditionalCocktailsData._traditional_cocktails_data, typeof(TraditionalCocktailDescriptions), ingredients),
                    .. this.GetCocktails(ModernCocktailsData._modern_cocktails_data, typeof(ModernCocktailDescriptions), ingredients)];

                availableCocktails = [.. availableCocktails.OrderBy(x => x.Title)];
                cocktails.AddRange(availableCocktails);
            }
        }

        return cocktails;
    }

    private List<Cocktail> GetCocktails(byte[] jsonResx, Type cocktailDescriptionsResx, List<Ingredient> ingredients)
    {
        List<Cocktail> cocktails = [];

        var json = Encoding.UTF8.GetString(jsonResx);

        // set traditional cocktails
        cocktails.AddRange(JsonSerializer.Deserialize<List<Cocktail>>(json, jsonSerializerOptions));

        cocktails.ForEach(x =>
        {
            x.SetContent(ProcessMarkdown(x, cocktailsApiConfig.Value, cocktailDescriptionsResx));

            x.Images.ForEach(i => i.SetUri($"{cocktailsApiConfig.Value.BaseImageUri}/{i.Uri}"));

            x.Ingredients.ForEach(ci => ci.SetBaseIngredient(ingredients.FirstOrDefault(i => i.Id == ci.IngredientId)));
        });

        return cocktails;
    }

    private static string ProcessMarkdown(Cocktail cocktail, CocktailsApiConfig cocktailsApiConfig, Type cocktailDescriptionsResx)
    {
        var markdownBytes = (byte[])cocktailDescriptionsResx
            .GetProperties()
            .Where(x => x.Name == cocktail.ContentFile)
            .FirstOrDefault()
            .GetGetMethod(false)
            .Invoke(null, null);

        var markdown = RemoveBOM(Encoding.UTF8.GetString(markdownBytes));

        var mainImage = cocktail.Images?.FirstOrDefault(x => x.Type == CocktailImageType.Main);

        if (mainImage != null)
        {
            markdown = markdown
                .Replace("{baseImageUrl}", cocktailsApiConfig.BaseImageUri)
                .Replace("{mainImageName}", mainImage.Uri);
        }

        return markdown;
    }

    private static string RemoveBOM(string content)
    {
        if (content.StartsWith(bomMarkUtf8, StringComparison.Ordinal))
        {
            return content[bomMarkUtf8.Length..];
        }

        return content;
    }
}

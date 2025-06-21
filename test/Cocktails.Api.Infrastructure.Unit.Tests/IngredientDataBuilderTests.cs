namespace Cocktails.Api.Infrastructure.Unit.Tests;

using Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using Cocktails.Api.Domain.Aggregates.IngredientAggregate;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class IngredientDataBuilderTests : ServiceTestBase
{
    private readonly static JsonSerializerOptions jsonSerializerOptions;

    static IngredientDataBuilderTests()
    {
        jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            AllowTrailingCommas = true
        };

        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    }

    //[Fact]
    //public void GenerateIngredients()
    //{
    //    // arrange
    //    var sp = this.SetupEnvironment();
    //    var cocktailsDataStore = sp.GetRequiredService<CocktailDataStore>();
    //    var cocktails = cocktailsDataStore.Cocktails;
    //    var ingredients = new List<Ingredient>();
    //    var totalIngredients = new List<CocktailIngredient>();
    //    var earliestDate = DateTimeOffset.Now;

    //    var rgx = new Regex("[^a-zA-Z0-9 -]");

    //    string idReplace(string val)
    //    {
    //        return rgx.Replace(val
    //            .ToLower()
    //            .Replace(" ", "-")
    //            .Replace(".", "-")
    //            .Replace("é", "e")
    //            .Replace("è", "e")
    //            .Replace("ç", "c")
    //            .Replace("ñ", "n"), "");
    //    }

    //    foreach (var item in cocktails)
    //    {
    //        foreach (var ing in item.Ingredients)
    //        {
    //            var id = idReplace(ing.Name);
    //            var filterVal = idReplace(ing.FilterValue);
    //            totalIngredients.Add(ing);

    //            if (!ingredients.Any(x => x.Name == ing.Name && x.Id == filterVal))
    //            {
    //                ingredients.Add(new Ingredient(
    //                    id: id,
    //                    name: ing.Name,
    //                    shelfDisplay: ing.FilterDisplay,
    //                    types: ing.Types,
    //                    applications: ing.Applications,
    //                    createdOn: earliestDate,
    //                    updatedOn: earliestDate));
    //            }
    //        }
    //    }

    //    totalIngredients.DistinctBy(x => x.Name).ToList().Should().HaveCount(ingredients.Count);

    //    var json = JsonSerializer.Serialize(ingredients.OrderBy(x => x.Name), jsonSerializerOptions);
    //    System.IO.File.WriteAllText(@"C:\temp\ingredients.json", json);
    //}
}

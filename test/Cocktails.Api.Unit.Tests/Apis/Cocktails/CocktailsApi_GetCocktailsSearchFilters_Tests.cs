namespace Cocktails.Api.Unit.Tests.Apis.Cocktails;

using Cezzi.Applications.Extensions;
using FluentAssertions;
using global::Cocktails.Api.Apis.Cockails;
using global::Cocktails.Api.Application.Concerns.Cocktails;
using global::Cocktails.Api.Application.Concerns.Cocktails.Models;
using global::Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using global::Cocktails.Api.Domain.Aggregates.IngredientAggregate;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class CocktailsApi_GetCocktailsSearchFilters_Tests : ServiceTestBase
{
    [Fact]
    public async Task GetCocktailsSearch_Returns_Correct_Data()
    {
        // arrange
        var sp = this.SetupEnvironment();
        var allCocktails = sp.GetRequiredService<ICocktailRepository>().CachedItems.ToList();
        var services = GetAsParameterServices<CocktailsServices>(sp);
        var ingredients = allCocktails
            .SelectMany(x => x.Ingredients)
            .Where(x => x.ParentIngredientId == null)
            .ToList();

        // act
        var response = (await CocktailsApi.GetCocktailIngredientFilters(services))?.Result as Ok<CocktailIngredientFiltersRs>;
        var result = response?.Value;

        // assert
        result.Should().NotBeNull();

        result.Glassware.Should().BeEquivalentTo([.. allCocktails
            .SelectMany(x => x.Glassware)
            .Select(x => Enum.Parse<GlasswareType>(x, true))
            .Where(x => x != GlasswareType.None)
            .Select(x => x.GetAttributeOfType<DisplayAttribute>())
            .Select(x => x?.Name.ToString() ?? string.Empty)
            .Distinct()
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .OrderBy(x => x)
            .Select(x => new IngredientFilterModel($"glassware-{x.ToLower().Replace(" ", "-")}", x))]);

        result.Spirits.Should().BeEquivalentTo([.. ingredients
            .Where(x => x.Types.Contains(IngredientType.Spirit.ToString(), StringComparer.OrdinalIgnoreCase))
            .Select(x => new IngredientFilter(x.IngredientId, IngredientType.Spirit, x.BaseName))
            .DistinctBy(x => x.Name)
            .OrderBy(x => x.Name)
            .Select(x => new IngredientFilterModel($"{x.Type.ToString().ToLower()}-{x.Id.ToLower()}", x.Name))]);

        result.Liqueurs.Should().BeEquivalentTo([.. ingredients
            .Where(x => x.Types.Contains(IngredientType.Liqueur.ToString(), StringComparer.OrdinalIgnoreCase))
            .Select(x => new IngredientFilter(x.IngredientId, IngredientType.Liqueur, x.BaseName))
            .DistinctBy(x => x.Name)
            .OrderBy(x => x.Name)
            .Select(x => new IngredientFilterModel($"{x.Type.ToString().ToLower()}-{x.Id.ToLower()}", x.Name))]);

        result.Fruits.Should().BeEquivalentTo([.. ingredients
            .Where(x => x.Types.Contains(IngredientType.Fruit.ToString(), StringComparer.OrdinalIgnoreCase))
            .Select(x => new IngredientFilter(x.IngredientId, IngredientType.Fruit, x.BaseName))
            .DistinctBy(x => x.Name)
            .OrderBy(x => x.Name)
            .Select(x => new IngredientFilterModel($"{x.Type.ToString().ToLower()}-{x.Id.ToLower()}", x.Name))]);

        result.Vegetables.Should().BeEquivalentTo([.. ingredients
            .Where(x => x.Types.Contains(IngredientType.Vegetable.ToString(), StringComparer.OrdinalIgnoreCase))
            .Select(x => new IngredientFilter(x.IngredientId, IngredientType.Vegetable, x.BaseName))
            .DistinctBy(x => x.Name)
            .OrderBy(x => x.Name)
            .Select(x => new IngredientFilterModel($"{x.Type.ToString().ToLower()}-{x.Id.ToLower()}", x.Name))]);

        result.HerbsAndFlowers.Should().BeEquivalentTo([.. ingredients
            .Where(x => x.Types.Contains(IngredientType.Herb.ToString(), StringComparer.OrdinalIgnoreCase) || x.Types.Contains(IngredientType.Flowers.ToString(), StringComparer.OrdinalIgnoreCase))
            .Select(x => new IngredientFilter(x.IngredientId, x.Types.Contains(IngredientType.Herb.ToString(), StringComparer.OrdinalIgnoreCase) ? IngredientType.Herb : IngredientType.Flowers, x.BaseName))
            .DistinctBy(x => x.Name)
            .OrderBy(x => x.Name)
            .Select(x => new IngredientFilterModel($"{x.Type.ToString().ToLower()}-{x.Id.ToLower()}", x.Name))]);

        result.SyrupsAndSauces.Should().BeEquivalentTo([.. ingredients
            .Where(x => x.Types.Contains(IngredientType.Syrup.ToString(), StringComparer.OrdinalIgnoreCase) || x.Types.Contains(IngredientType.Sauce.ToString(), StringComparer.OrdinalIgnoreCase))
            .Select(x => new IngredientFilter(x.IngredientId, x.Types.Contains(IngredientType.Syrup.ToString(), StringComparer.OrdinalIgnoreCase) ? IngredientType.Syrup : IngredientType.Sauce, x.BaseName))
            .DistinctBy(x => x.Name)
            .OrderBy(x => x.Name)
            .Select(x => new IngredientFilterModel($"{x.Type.ToString().ToLower()}-{x.Id.ToLower()}", x.Name))]);

        result.Bitters.Should().BeEquivalentTo([.. ingredients
            .Where(x => x.Types.Contains(IngredientType.Bitters.ToString(), StringComparer.OrdinalIgnoreCase))
            .Select(x => new IngredientFilter(x.IngredientId, IngredientType.Bitters, x.BaseName))
            .DistinctBy(x => x.Name)
            .OrderBy(x => x.Name)
            .Select(x => new IngredientFilterModel($"{x.Type.ToString().ToLower()}-{x.Id.ToLower()}", x.Name))]);

        result.Proteins.Should().BeEquivalentTo([.. ingredients
            .Where(x => x.Types.Contains(IngredientType.Protein.ToString(), StringComparer.OrdinalIgnoreCase))
            .Select(x => new IngredientFilter(x.IngredientId, IngredientType.Protein, x.BaseName))
            .DistinctBy(x => x.Name)
            .OrderBy(x => x.Name)
            .Select(x => new IngredientFilterModel($"{x.Type.ToString().ToLower()}-{x.Id.ToLower()}", x.Name))]);

        result.Juices.Should().BeEquivalentTo([.. ingredients
            .Where(x => x.Types.Contains(IngredientType.Juice.ToString(), StringComparer.OrdinalIgnoreCase))
            .Select(x => new IngredientFilter(x.IngredientId, IngredientType.Juice, x.BaseName))
            .DistinctBy(x => x.Name)
            .OrderBy(x => x.Name)
            .Select(x => new IngredientFilterModel($"{x.Type.ToString().ToLower()}-{x.Id.ToLower()}", x.Name))]);

        result.Dilutions.Should().BeEquivalentTo([.. ingredients
            .Where(x => x.Types.Contains(IngredientType.Dilution.ToString(), StringComparer.OrdinalIgnoreCase))
            .Select(x => new IngredientFilter(x.IngredientId, IngredientType.Dilution, x.BaseName))
            .DistinctBy(x => x.Name)
            .OrderBy(x => x.Name)
            .Select(x => new IngredientFilterModel($"{x.Type.ToString().ToLower()}-{x.Id.ToLower()}", x.Name))]);

        result.BeerWineChampagne.Should().BeEquivalentTo([.. ingredients
            .Where(x => x.Types.Contains(IngredientType.Beer.ToString(), StringComparer.OrdinalIgnoreCase) || x.Types.Contains(IngredientType.Wine.ToString(), StringComparer.OrdinalIgnoreCase) || x.Types.Contains(IngredientType.Champagne.ToString(), StringComparer.OrdinalIgnoreCase))
            .Select(x => new IngredientFilter(x.IngredientId, x.Types.Contains(IngredientType.Beer.ToString(), StringComparer.OrdinalIgnoreCase) ? IngredientType.Beer : x.Types.Contains(IngredientType.Wine.ToString(), StringComparer.OrdinalIgnoreCase) ? IngredientType.Wine : IngredientType.Champagne, x.BaseName))
            .DistinctBy(x => x.Name)
            .OrderBy(x => x.Name)
            .Select(x => new IngredientFilterModel($"{x.Type.ToString().ToLower()}-{x.Id.ToLower()}", x.Name))]);

        result.Eras.Should().BeEquivalentTo([.. allCocktails
            .SelectMany(x => x.Eras)
            .Select(x => x)
            .Distinct()
            .OrderBy(x => x)
            .Select(x => new IngredientFilterModel($"era-{x.ToLower().Replace(" ", "-")}", x))]);
    }
}

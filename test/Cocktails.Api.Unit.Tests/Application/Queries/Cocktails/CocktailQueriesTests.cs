namespace Cocktails.Api.Unit.Tests.Application.Queries.Cocktails;

using FluentAssertions;
using global::Cocktails.Api.Apis.Cockails;
using global::Cocktails.Api.Application.Concerns.Cocktails.Models;
using global::Cocktails.Api.Application.Utilities;
using global::Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using global::Cocktails.Api.Domain.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Xunit;

public class CocktailQueriesTests : ServiceTestBase
{
    private const string fiftyithCocktailId = "mai-tai";

    [Fact]
    public async Task GetCocktail_ShouldReturnCocktailItem()
    {
        // arrange
        var sp = this.SetupEnvironment();
        var repo = sp.GetRequiredService<ICocktailRepository>();
        var services = GetAsParameterServices<CocktailsServices>(sp);
        var config = sp.GetRequiredService<IOptions<CocktailsApiConfig>>().Value;
        var webconfig = sp.GetRequiredService<IOptions<CocktailsWebConfig>>().Value;
        var allCocktails = repo.CachedItems.ToList();

        var expectedCocktail = allCocktails.First(x => x.Id == fiftyithCocktailId);

        // act
        var result = await services.Queries.GetCocktail(fiftyithCocktailId, default);

        // assert
        AssertionHelpers.AssertCocktailModelMatches(expectedCocktail, result?.Item, config);
    }

    [Theory]
    [InlineData("bees-knees")]
    [InlineData("Bees-knees")]
    public async Task getcocktail__returns_case_insensitve_lookup_byid(string id)
    {
        // arrange
        var sp = this.SetupEnvironment();
        var services = GetAsParameterServices<CocktailsServices>(sp);

        // act
        var cocktail = await services.Queries.GetCocktail(id);

        // assert
        AssertionHelpers.AssertBeesKneesCocktail(sp, cocktail?.Item);
    }

    [Theory]
    [InlineData("whud-a-burger")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task getcocktail__returns_null_when_cocktail_does_not_exist(string id)
    {
        // arrange
        var sp = this.SetupEnvironment();
        var services = GetAsParameterServices<CocktailsServices>(sp);

        var cocktail = await services.Queries.GetCocktail(id);

        cocktail.Should().BeNull();
    }
}

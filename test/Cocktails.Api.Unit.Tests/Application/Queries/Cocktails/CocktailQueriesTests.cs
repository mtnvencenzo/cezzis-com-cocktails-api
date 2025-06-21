namespace Cocktails.Api.Unit.Tests.Application.Queries.Cocktails;

using FluentAssertions;
using global::Cocktails.Api.Apis.Cockails;
using global::Cocktails.Api.Application.Concerns.Cocktails;
using global::Cocktails.Api.Application.Concerns.Cocktails.Models;
using global::Cocktails.Api.Application.Concerns.Cocktails.Queries;
using global::Cocktails.Api.Application.Utilities;
using global::Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using global::Cocktails.Api.Domain.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Xunit;

public class CocktailQueriesTests : ServiceTestBase
{
    private const string firstCocktailId = "absinthe-frappe";
    private const string twentyithCocktailId = "clover-club";
    private const string fiftyithCocktailId = "mojito";
    private readonly Mock<ICocktailQueries> cocktailQueriesMock = new();

    public CocktailQueriesTests()
    {
        //cocktailQueriesMock.SetupPropertys(x => x.ite, firstCocktailId);
    }

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
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task getcocktails__empty_freetext_respects_default_skip_take_values(string freetext)
    {
        // arrange
        var sp = this.SetupEnvironment();
        var repo = sp.GetRequiredService<ICocktailRepository>();
        var services = GetAsParameterServices<CocktailsServices>(sp);
        var config = sp.GetRequiredService<IOptions<CocktailsApiConfig>>().Value;
        var webconfig = sp.GetRequiredService<IOptions<CocktailsWebConfig>>().Value;
        var allCocktails = repo.CachedItems.ToList();
        CocktailDataIncludeModel[] includes = [];

        var first = repo.CachedItems.First(x => x.Id == firstCocktailId);
        var twentith = repo.CachedItems.First(x => x.Id == twentyithCocktailId);

        // act
        var rs = await services.Queries.GetCocktailsList(freeText: freetext, include: includes);

        // assert
        rs.Should().NotBeNull();
        rs.Items.Should().NotBeNullOrEmpty();
        rs.Items.Should().HaveCount(20);

        // no dups
        rs.Items
            .GroupBy(x => x.Id)
            .Should()
            .HaveCount(20);

        AssertionHelpers.AssertCocktailListItem(first, rs.Items.First(), includes);
        AssertionHelpers.AssertCocktailListItem(twentith, rs.Items.Last(), includes);
    }

    [Theory]
    [InlineData(0, 50)]
    [InlineData(-1, 51)]
    [InlineData(-100, 5001)]
    public async Task getcocktails__empty_freetext_uses_default_skip_take_when_out_of_bounds(int skip, int take)
    {
        // arrange
        var sp = this.SetupEnvironment();
        var repo = sp.GetRequiredService<ICocktailRepository>();
        var services = GetAsParameterServices<CocktailsServices>(sp);
        var config = sp.GetRequiredService<IOptions<CocktailsApiConfig>>().Value;
        var webconfig = sp.GetRequiredService<IOptions<CocktailsWebConfig>>().Value;
        var allCocktails = repo.CachedItems.ToList();

        var first = repo.CachedItems.First(x => x.Id == firstCocktailId);
        var fiftyith = repo.CachedItems.First(x => x.Id == fiftyithCocktailId);
        CocktailDataIncludeModel[] includes = [CocktailDataIncludeModel.mainImages, CocktailDataIncludeModel.searchTiles, CocktailDataIncludeModel.descriptiveTitle];

        // act
        var rs = await services.Queries.GetCocktailsList(freeText: string.Empty, skip: skip, take: take, include: includes);

        // assert
        rs.Should().NotBeNull();
        rs.Items.Should().NotBeNull();
        rs.Items.Should().HaveCount(50);

        // no dups
        rs.Items
            .GroupBy(x => x.Id)
            .Should()
            .HaveCount(50);

        AssertionHelpers.AssertCocktailListItem(first, rs.Items.First(), includes);
        AssertionHelpers.AssertCocktailListItem(fiftyith, rs.Items.Last(), includes);
    }

    [Theory]
    [InlineData(3, 1)]
    [InlineData(3, 2)]
    public async Task getcocktails__empty_freetext_uses_skip_take(int skip, int take)
    {
        // arrange
        var sp = this.SetupEnvironment();
        var repo = sp.GetRequiredService<ICocktailRepository>();
        var services = GetAsParameterServices<CocktailsServices>(sp);
        var config = sp.GetRequiredService<IOptions<CocktailsApiConfig>>().Value;
        var webconfig = sp.GetRequiredService<IOptions<CocktailsWebConfig>>().Value;
        var allCocktails = repo.CachedItems.ToList();
        CocktailDataIncludeModel[] includes = [CocktailDataIncludeModel.searchTiles, CocktailDataIncludeModel.descriptiveTitle];

        var first = allCocktails[3];
        var second = allCocktails[4];

        // act
        var rs = await services.Queries.GetCocktailsList(freeText: string.Empty, skip: skip, take: take, include: includes);

        // assert
        rs.Should().NotBeNull();
        rs.Items.Should().NotBeNull();
        rs.Items.Should().HaveCount(take);

        // no dups
        rs.Items
            .GroupBy(x => x.Id)
            .Should()
            .HaveCount(take);

        AssertionHelpers.AssertCocktailListItem(first, rs.Items.First(), includes);

        if (take > 1)
        {
            AssertionHelpers.AssertCocktailListItem(second, rs.Items.Last(), includes);
        }
    }

    [Theory]
    [InlineData(3, 1)]
    [InlineData(3, 2)]
    public async Task getcocktails__partial_match_freetext_uses_skip_take(int skip, int take)
    {
        // arrange
        var sp = this.SetupEnvironment();
        var repo = sp.GetRequiredService<ICocktailRepository>();
        var services = GetAsParameterServices<CocktailsServices>(sp);
        var config = sp.GetRequiredService<IOptions<CocktailsApiConfig>>().Value;
        var webconfig = sp.GetRequiredService<IOptions<CocktailsWebConfig>>().Value;
        var allCocktails = repo.CachedItems.ToList();

        var first = allCocktails.First(x => x.Id == "bijou");
        var second = allCocktails.First(x => x.Id == "black-velvet");
        CocktailDataIncludeModel[] includes = [CocktailDataIncludeModel.mainImages];

        // act
        var rs = await services.Queries.GetCocktailsList(freeText: "b", skip: skip, take: take, include: includes);

        // assert
        rs.Should().NotBeNull();
        rs.Items.Should().NotBeNull();
        rs.Items.Should().HaveCount(take);

        // no dups
        rs.Items
            .GroupBy(x => x.Id)
            .Should()
            .HaveCount(take);

        AssertionHelpers.AssertCocktailListItem(first, rs.Items.First(), includes);

        if (take > 1)
        {
            AssertionHelpers.AssertCocktailListItem(second, rs.Items.Last(), includes);
        }
    }

    [Theory]
    [InlineData(3, 1)]
    [InlineData(3, 2)]
    public async Task getcocktails__no_match_freetext_respects_skip_take_even_though_doesnt_matter(int skip, int take)
    {
        // arrange
        var sp = this.SetupEnvironment();
        var services = GetAsParameterServices<CocktailsServices>(sp);

        // act
        var rs = await services.Queries.GetCocktailsList(freeText: "whad-up-burger", skip: skip, take: take);

        // assert
        rs.Should().NotBeNull();
        rs.Items.Should().NotBeNull();
        rs.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task getcocktails__no_match_freetext_respects_default_skip_take_even_though_doesnt_matter()
    {
        // arrange
        var sp = this.SetupEnvironment();
        var services = GetAsParameterServices<CocktailsServices>(sp);

        // act
        var rs = await services.Queries.GetCocktailsList(freeText: "whad-up-burger");

        // assert
        rs.Should().NotBeNull();
        rs.Items.Should().NotBeNull();
        rs.Items.Should().BeEmpty();
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

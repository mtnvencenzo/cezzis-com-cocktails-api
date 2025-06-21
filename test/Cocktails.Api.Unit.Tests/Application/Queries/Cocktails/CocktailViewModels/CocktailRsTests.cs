namespace Cocktails.Api.Unit.Tests.Application.Queries.Cocktails.CocktailViewModels;

using AutoBogus;
using FluentAssertions;
using global::Cocktails.Api.Application.Concerns.Cocktails;
using global::Cocktails.Api.Application.Concerns.Cocktails.Models;
using Xunit;

public class CocktailRsTests
{
    [Fact]
    public void CocktailRs_Item_ShouldBeSet()
    {
        // arrange
        var cocktailModel = AutoFaker.Generate<CocktailModel>();

        // act
        var cocktailRs = new CocktailRs(Item: cocktailModel);

        // assert
        cocktailRs.Item.Should().Be(cocktailModel);
    }

    [Fact]
    public void CocktailRs_Item_ShouldBeNullByDefault()
    {
        // arrange
        var cocktailRs = new CocktailRs(Item: null);

        // assert
        cocktailRs.Item.Should().BeNull();
    }
}

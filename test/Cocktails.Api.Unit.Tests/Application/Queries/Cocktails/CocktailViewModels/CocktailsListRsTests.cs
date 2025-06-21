namespace Cocktails.Api.Unit.Tests.Application.Queries.Cocktails.CocktailViewModels;

using AutoBogus;
using FluentAssertions;
using global::Cocktails.Api.Application.Concerns.Cocktails;
using global::Cocktails.Api.Application.Concerns.Cocktails.Models;
using System.Collections.Generic;
using Xunit;

public class CocktailsListRsTests
{
    [Fact]
    public void CocktailsListRs_Items_ShouldBeInitialized()
    {
        // Arrange
        var rs = new CocktailsListRs(Items: []);

        // Act

        // Assert
        rs.Items.Should().NotBeNull();
        rs.Items.Should().BeEmpty();
    }

    [Fact]
    public void CocktailsListRs_Items_ShouldBeSettable()
    {
        // Arrange
        var items = new List<CocktailsListModel>
        {
            AutoFaker.Generate<CocktailsListModel>(),
            AutoFaker.Generate<CocktailsListModel>(),
            AutoFaker.Generate<CocktailsListModel>()
        };

        // Act
        var rs = new CocktailsListRs(Items: items);

        // Assert
        rs.Items.Should().BeEquivalentTo(items);
    }
}

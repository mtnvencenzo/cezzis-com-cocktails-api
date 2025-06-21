namespace Cocktails.Api.Unit.Tests.Application.Queries.Cocktails.CocktailViewModels;

using AutoBogus;
using FluentAssertions;
using global::Cocktails.Api.Application.Concerns.Cocktails;
using global::Cocktails.Api.Application.Concerns.Cocktails.Models;
using Xunit;

public class CocktailsListModelTests
{
    [Fact]
    public void cocktailsListModel_Id_ShouldBeSettableAndRetrievable()
    {
        // arrange
        var expectedId = "123";

        // act
        var cocktailListModel = new AutoFaker<CocktailsListModel>()
            .RuleFor(x => x.Id, expectedId)
            .Generate();

        // assert
        cocktailListModel.Id.Should().Be(expectedId);
    }

    [Fact]
    public void cocktailsListModel_Title_ShouldBeSettableAndRetrievable()
    {
        // arrange
        var expectedTitle = "Mocktail";

        // act
        var cocktailListModel = new AutoFaker<CocktailsListModel>()
            .RuleFor(x => x.Title, expectedTitle)
            .Generate();

        // assert
        cocktailListModel.Title.Should().Be(expectedTitle);
    }

    [Fact]
    public void cocktailsListModel_IsIba_ShouldBeSettableAndRetrievable()
    {
        // arrange
        var expectedIsIba = true;

        // act
        var cocktailListModel = new AutoFaker<CocktailsListModel>()
            .RuleFor(x => x.IsIba, expectedIsIba)
            .Generate();

        // assert
        cocktailListModel.IsIba.Should().Be(expectedIsIba);
    }
}

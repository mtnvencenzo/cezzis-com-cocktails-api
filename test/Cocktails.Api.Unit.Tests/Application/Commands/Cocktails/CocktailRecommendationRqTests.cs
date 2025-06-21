namespace Cocktails.Api.Unit.Tests.Application.Commands.Cocktails;

using FluentAssertions;
using global::Cocktails.Api.Application.Concerns.Accounts.Models;
using System;
using Xunit;

public class CocktailRecommendationRqTests
{
    [Fact]
    public void CocktailRecommendationRq_Property_Defaults()
    {
        // arrange
        var item = new CocktailRecommendationRq(Recommendation: null, VerificationCode: null);

        // assert
        item.VerificationCode.Should().BeNull();
        item.Recommendation.Should().BeNull();
    }

    [Fact]
    public void CocktailRecommendationRq_VerificationCodeProperty_ShouldGetAndSetCorrectly()
    {
        // arrange
        var expectedVerificationCode = "123";

        // act
        var item = new CocktailRecommendationRq(Recommendation: null, VerificationCode: expectedVerificationCode);
        var actuaVerificiationCode = item.VerificationCode;

        // assert
        actuaVerificiationCode.Should().Be(expectedVerificationCode);
    }

    [Fact]
    public void CocktailRecommendationRq_RecommendationProperty_ShouldGetAndSetCorrectly()
    {
        // arrange
        var expectedRecommendation = new CocktailRecommendationModel
        (
            Directions: Guid.NewGuid().ToString(),
            Ingredients: Guid.NewGuid().ToString(),
            Name: Guid.NewGuid().ToString()
        );

        // act
        var item = new CocktailRecommendationRq(Recommendation: expectedRecommendation, VerificationCode: null);
        var actualRecommendation = item.Recommendation;

        // assert
        expectedRecommendation.Should().BeEquivalentTo(actualRecommendation);
    }
}

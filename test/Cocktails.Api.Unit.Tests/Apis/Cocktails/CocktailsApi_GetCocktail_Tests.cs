namespace Cocktails.Api.Unit.Tests.Apis.Cocktails;

using global::Cocktails.Api.Apis.Cockails;
using global::Cocktails.Api.Application.Concerns.Cocktails;
using global::Cocktails.Api.Application.Concerns.Cocktails.Models;
using global::Cocktails.Api.Application.Utilities;
using global::Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using global::Cocktails.Api.Domain.Config;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

public class CocktailsApi_GetCocktail_Tests : ServiceTestBase
{
    [Fact]
    public async Task getcocktail__returns_correct_data()
    {
        // arrange
        var sp = this.SetupEnvironment();
        var repo = sp.GetRequiredService<ICocktailRepository>();
        var services = GetAsParameterServices<CocktailsServices>(sp);
        var config = sp.GetRequiredService<IOptions<CocktailsApiConfig>>();
        var allCocktails = repo.CachedItems.ToList();

        // act
        foreach (var cocktail in allCocktails)
        {
            try
            {
                if (cocktail.Id == "sidecar")
                {
                }

                var response = (await CocktailsApi.GetCocktail(cocktail.Id, services))?.Result as Ok<CocktailRs>;

                // assert
                AssertionHelpers.AssertCocktailModelMatches(cocktail, response?.Value?.Item, config.Value);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error processing cocktail {cocktail.Id}: {ex.Message}", ex);
            }
        }
    }
}

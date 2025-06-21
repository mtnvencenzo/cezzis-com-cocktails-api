namespace Cocktails.Api.Unit.Tests.Apis.Cocktails;

using FluentAssertions;
using global::Cocktails.Api.Apis.Cockails;
using global::Cocktails.Api.Application.Concerns.Cocktails;
using global::Cocktails.Api.Application.Concerns.Cocktails.Models;
using global::Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using global::Cocktails.Api.Domain.Config;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class CocktailsApi_GetCocktailsSearch_Tests : ServiceTestBase
{
    [Theory]
    [InlineData(0, 50)]
    [InlineData(30, 50)]
    public async Task GetCocktailsList_Returns_Correct_Data(int skip, int take)
    {
        // arrange
        var sp = this.SetupEnvironment();
        var repo = sp.GetRequiredService<ICocktailRepository>();
        var services = GetAsParameterServices<CocktailsServices>(sp);
        var config = sp.GetRequiredService<IOptions<CocktailsApiConfig>>().Value;
        var allCocktails = repo.CachedItems.ToList();

        // act
        var response = (await CocktailsApi.GetCocktailsList(
            freeText: string.Empty,
            skip: skip,
            take: take,
            cocktailsServices: services,
            include: [
                CocktailDataIncludeModel.searchTiles,
                CocktailDataIncludeModel.mainImages,
                CocktailDataIncludeModel.descriptiveTitle
            ]))?.Result as Ok<CocktailsListRs>;

        var result = response?.Value;

        // assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(take);

        foreach (var item in result.Items)
        {
            var cocktail = allCocktails.FirstOrDefault(x => x.Id == item.Id);
            var ingredients = cocktail.GetIngredientsMarkDownDescription();

            item.Id.Should().Be(cocktail.Id);
            item.Title.Should().Be(cocktail.Title);
            item.IsIba.Should().Be(cocktail.IsIba);
            item.Ingredients.Should().HaveSameCount(cocktail.Ingredients);
            item.DescriptiveTitle.Should().Be(cocktail.DescriptiveTitle);
            item.SearchTiles.Should().HaveSameCount(cocktail.Images.Where(x => x.Type == CocktailImageType.SearchTile));
            item.MainImages.Should().HaveSameCount(cocktail.Images.Where(x => x.Type == CocktailImageType.Main));

            foreach (var img in item.MainImages)
            {
                img.Should().StartWith($"{config.BaseImageUri}/");
            }

            foreach (var img in item.SearchTiles)
            {
                img.Should().StartWith($"{config.BaseImageUri}/");
            }
        }
    }
}

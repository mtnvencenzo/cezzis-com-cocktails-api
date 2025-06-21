namespace Cocktails.Api.Unit.Tests.Apis.Cocktails;

using FluentAssertions;
using global::Cocktails.Api.Apis.Cockails;
using global::Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using global::Cocktails.Api.Domain.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

public class CocktailsApi_GetCocktailsSiteMap_Tests : ServiceTestBase
{
    [Fact]
    public async Task GetCocktailsListSiteMap_Returns_Correct_Data()
    {
        //arrange
        var sp = this.SetupEnvironment();
        var repo = sp.GetRequiredService<ICocktailRepository>();
        var services = GetAsParameterServices<CocktailsServices>(sp);
        var config = sp.GetRequiredService<IOptions<CocktailsApiConfig>>().Value;
        var webconfig = sp.GetRequiredService<IOptions<CocktailsWebConfig>>().Value;
        var allCocktails = repo.CachedItems.ToList();

        // act
        var response = (await CocktailsApi.GetCocktailsSiteMap(services));

        //assert
        response.ContentType.Should().Be("text/xml");
        response.StatusCode.Should().Be(StatusCodes.Status200OK);
        response.ResponseContent.Should().NotBeNullOrWhiteSpace();

        using var reader = new StringReader(response.ResponseContent);

        var ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        var nsImage = "http://www.google.com/schemas/sitemap-image/1.1";
        var locRoot = webconfig.SiteMap.CockailsPageFormat.Replace(":id", string.Empty);

        var el = XElement.Load(reader);
        var urls = el.Elements(XName.Get("url", ns));
        urls.Should().HaveCount(allCocktails.Count);

        foreach (var url in urls)
        {
            var loc = url.Element(XName.Get("loc", ns)).Value;
            loc.Should().StartWith(locRoot);

            var cocktail = allCocktails.FirstOrDefault(x => x.Id == loc.Replace(locRoot, ""));
            cocktail.Should().NotBeNull();

            var lastmod = url.Element(XName.Get("lastmod", ns)).Value;
            lastmod.Should().Be(cocktail.ModifiedOn.ToString("o"));

            var changefreq = url.Element(XName.Get("changefreq", ns)).Value;
            changefreq.Should().Be("daily");

            var priority = url.Element(XName.Get("priority", ns)).Value;
            priority.Should().Be("0.8");

            var images = url.Elements(XName.Get("image", nsImage));
            images.Should().ContainSingle();

            var firstImage = images.First();
            var imageLoc = firstImage.Element(XName.Get("loc", nsImage)).Value;
            imageLoc.Should().Be(cocktail.Images.First(x => x.Type == CocktailImageType.Main).Uri);
        }
    }
}

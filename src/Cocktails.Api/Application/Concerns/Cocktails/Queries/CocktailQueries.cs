namespace Cocktails.Api.Application.Concerns.Cocktails.Queries;

using Cezzi.Applications.Extensions;
using Cocktails;
using global::Cocktails.Api.Application.Concerns.Cocktails.Models;
using global::Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using global::Cocktails.Api.Domain.Aggregates.IngredientAggregate;
using global::Cocktails.Api.Domain.Config;
using global::Cocktails.Api.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

public class CocktailQueries(
    ICocktailRepository cocktailRepository,
    IOptions<CocktailsWebConfig> cocktailWebConfig,
    ISearchClient searchClient) : ICocktailQueries
{
    private readonly static Dictionary<string, GlasswareType> GlasswareDisplayNameMapping;

    static CocktailQueries()
    {
        GlasswareDisplayNameMapping = Enum.GetNames<GlasswareType>()
            .Where(x => x != GlasswareType.None.ToString())
            .Select(x => new
            {
                attr = Enum.Parse<GlasswareType>(x).GetAttributeOfType<DisplayAttribute>(),
                enumVal = Enum.Parse<GlasswareType>(x)
            })
            .Where(x => x.attr != null)
            .ToDictionary(x => x.attr.Name.ToLower(), x => x.enumVal);
    }

    public Task<CocktailRs> GetCocktail(
        string id,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult(null as CocktailRs);
        }

        var item = cocktailRepository.CachedItems.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));

        if (item == null)
        {
            return Task.FromResult(null as CocktailRs);
        }

        var glasswareConverter = TypeDescriptor.GetConverter(typeof(GlasswareTypeModel));
        var uomConverter = TypeDescriptor.GetConverter(typeof(UofMTypeModel));
        var ingredientTypeConverter = TypeDescriptor.GetConverter(typeof(IngredientTypeModel));
        var ingredientApplicationConverter = TypeDescriptor.GetConverter(typeof(IngredientApplicationModel));
        var preparationTypeConverter = TypeDescriptor.GetConverter(typeof(PreparationTypeModel));
        var requirementTypeConverter = TypeDescriptor.GetConverter(typeof(IngredientRequirementTypeModel));

        var rs = new CocktailRs
        (
            Item: item != null
                ? new CocktailModel
                (
                    Content: item.Content?
                        .Replace("{{ingredients}}", item.GetIngredientsMarkDownDescription())
                        .Replace("{{instructions}}", item.GetInstructionsMarkDownDescription())
                        .Replace("{{iba}}", item.GetIbaDescription()),
                    Glassware: [.. item.Glassware.Select(x => (GlasswareTypeModel)glasswareConverter.ConvertFrom(Enum.Parse<GlasswareType>(x, true)))],
                    Id: item.Id,
                    Serves: item.Serves,
                    Tags: [.. item.Eras],
                    SearchableTitles: [.. item.SearchableTitles],
                    Title: item.Title,
                    DescriptiveTitle: item.DescriptiveTitle,
                    Description: !string.IsNullOrWhiteSpace(item.Description)
                        ? item.Description
                        : item.DescriptiveTitle,
                    IsIba: item.IsIba,
                    ModifiedOn: item.ModifiedOn,
                    PublishedOn: item.PublishedOn,
                    PrepTimeMinutes: item.PrepTimeMinutes,
                    Rating: item.Rating != null
                        ? new CocktailRatingModel(item.Rating.OneStars, item.Rating.TwoStars, item.Rating.ThreeStars, item.Rating.FourStars, item.Rating.FiveStars, item.Rating.TotalStars, item.Rating.Rating, item.Rating.RatingCount)
                        : new CocktailRatingModel(0, 0, 0, 0, 0, 0, 0, 0),
                    Instructions: [.. item.Instructions.OrderBy(x => x.Order).Select(x => new InstructionStepModel
                    (
                        Display: x.DisplayValue,
                        Order: x.Order
                    ))],
                    MainImages: [.. item.Images.Where(x => x.Type == CocktailImageType.Main).Select(x => new CocktailImageModel
                    (
                        Uri: x.Uri,
                        Width: x.Width,
                        Height: x.Height
                    ))],
                    SearchTiles: [.. item.Images.Where(x => x.Type == CocktailImageType.SearchTile).Select(x => new CocktailImageModel2
                    (
                        Uri: x.Uri,
                        Width: x.Width,
                        Height: x.Height
                    ))],
                    Ingredients: [.. item.Ingredients.Select(x => new IngredientModel
                    (
                        UoM: (UofMTypeModel)uomConverter.ConvertFrom(x.UoM),
                        Name: x.Name,
                        Preparation: (PreparationTypeModel)preparationTypeConverter.ConvertFrom(x.Preparation),
                        Requirement: (IngredientRequirementTypeModel)requirementTypeConverter.ConvertFrom(x.Requirement),
                        Suggestions: x.Suggestions?.ToString() ?? string.Empty,
                        Types: [.. x.Types.Select(x => (IngredientTypeModel)ingredientTypeConverter.ConvertFrom(Enum.Parse<IngredientType>(x, true)))],
                        Applications: [.. x.Applications.Select(x => (IngredientApplicationModel)ingredientApplicationConverter.ConvertFrom(Enum.Parse<IngredientApplication>(x, true)))],
                        Units: x.Units,
                        Display: x.GetDisplayValue()
                    ))]
                ) : null
        );

        return Task.FromResult(rs);
    }

    public async Task<CocktailsListRs> GetCocktailsList(
        string freeText,
        int skip = 0,
        int take = 20,
        bool allowExcessiveTake = false,
        List<string> filters = null,
        CocktailDataIncludeModel[] include = null,
        string[] matches = null,
        bool matchExclusive = false,
        bool useSearchIndex = false,
        CancellationToken cancellationToken = default)
    {
        var useMatches = matches;

        if (matchExclusive && useMatches == null)
        {
            useMatches = [];
        }
        else if (!matchExclusive && matches != null && matches.Length == 0)
        {
            useMatches = null;
        }

        var cocktails = useSearchIndex && !string.IsNullOrWhiteSpace(freeText)
            ? await this.GetCocktails(string.Empty, 0, 2000, true, filters, useMatches, cancellationToken)
            : await this.GetCocktails(freeText, skip, take, allowExcessiveTake, filters, useMatches, cancellationToken);

        cocktails = useSearchIndex && !string.IsNullOrWhiteSpace(freeText)
            ? await this.FilterCocktailsWithSearchIndex(cocktails, freeText, skip, take, cancellationToken)
            : cocktails;

        var glasswareConverter = TypeDescriptor.GetConverter(typeof(GlasswareTypeModel));
        var uomConverter = TypeDescriptor.GetConverter(typeof(UofMTypeModel));
        var ingredientTypeConverter = TypeDescriptor.GetConverter(typeof(IngredientTypeModel));
        var ingredientApplicationConverter = TypeDescriptor.GetConverter(typeof(IngredientApplicationModel));
        var preparationTypeConverter = TypeDescriptor.GetConverter(typeof(PreparationTypeModel));
        var requirementTypeConverter = TypeDescriptor.GetConverter(typeof(IngredientRequirementTypeModel));

        return new CocktailsListRs
        (
            Items: [.. cocktails.Select(x => new CocktailsListModel
            (
                Id: x.Id,
                Title: x.Title,
                IsIba: x.IsIba,
                Rating: x.Rating != null ? x.Rating.Rating : 0,
                PrepTimeMinutes: x.PrepTimeMinutes,
                Glassware: [.. x.Glassware.Select(x => (GlasswareTypeModel)glasswareConverter.ConvertFrom(Enum.Parse<GlasswareType>(x, true)))],
                Serves: x.Serves,
                MainImages: include?.Contains(CocktailDataIncludeModel.mainImages) ?? false
                    ? x.Images.Where(x => x.Type == CocktailImageType.Main).Select(x => x.Uri).ToList()
                    : null,
                SearchTiles: include?.Contains(CocktailDataIncludeModel.searchTiles) ?? false
                    ? x.Images.Where(x => x.Type == CocktailImageType.SearchTile).Select(x => x.Uri).ToList()
                    : null,
                DescriptiveTitle: include?.Contains(CocktailDataIncludeModel.descriptiveTitle) ?? false
                    ? x.DescriptiveTitle ?? string.Empty
                    : null,
                Ingredients: [.. x.Ingredients.Select(x => new IngredientModel
                (
                    UoM: (UofMTypeModel)uomConverter.ConvertFrom(x.UoM),
                    Name: x.Name,
                    Preparation: (PreparationTypeModel)preparationTypeConverter.ConvertFrom(x.Preparation),
                    Requirement: (IngredientRequirementTypeModel)requirementTypeConverter.ConvertFrom(x.Requirement),
                    Suggestions: x.Suggestions?.ToString() ?? string.Empty,
                    Types: [.. x.Types.Select(x => (IngredientTypeModel)ingredientTypeConverter.ConvertFrom(Enum.Parse<IngredientType>(x, true)))],
                    Applications: [.. x.Applications.Select(x => (IngredientApplicationModel)ingredientApplicationConverter.ConvertFrom(Enum.Parse<IngredientApplication>(x, true)))],
                    Units: x.Units,
                    Display: x.GetDisplayValue()
                ))]
            ))]
        );
    }

    private async Task<List<Cocktail>> FilterCocktailsWithSearchIndex(
        List<Cocktail> cocktails,
        string freeText,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        var searchResults = await searchClient.SearchAsync(cocktails, freeText, skip, take, cancellationToken: cancellationToken);

        return searchResults;
    }

    public Task<string> GetCocktailsSiteMap(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult<string>(null);
        }

        var items = cocktailRepository.CachedItems
            .Where(x => x.Images.FirstOrDefault(x => x.Type == CocktailImageType.Main) != null)
            .Select(x => new
            {
                x.Id,
                x.ModifiedOn,
                ImageUri = x.Images.FirstOrDefault(x => x.Type == CocktailImageType.Main).Uri,
            });

        var cocktailUrls = items.AsEnumerable().Select(x =>
        {
            var imageXml = x.ImageUri != null
                ? $@"
            <image:image>
                <image:loc>{x.ImageUri}</image:loc>
            </image:image>"
                : "";

            return $@"
        <url>
            <loc>{cocktailWebConfig.Value.SiteMap.CockailsPageFormat.Replace(":id", x.Id)}</loc>
            <lastmod>{x.ModifiedOn:o}</lastmod>
            <changefreq>daily</changefreq>
            <priority>0.8</priority>{imageXml}
        </url>";
        }).ToList();

        var sitemap = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
    <urlset
        xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"" xmlns:image=""http://www.google.com/schemas/sitemap-image/1.1"">{string.Join("", cocktailUrls)}
    </urlset>
    ";

        return Task.FromResult(sitemap);
    }

    public Task<CocktailIngredientFiltersRs> GetCocktailIngredientFilters(CancellationToken cancellationToken = default)
    {
        var allCocktails = cocktailRepository.CachedItems.ToList();

        var ingredients = allCocktails
            .SelectMany(x => x.Ingredients)
            .Where(x => x.ParentIngredientId == null)
            .ToList();

        var filters = new CocktailIngredientFiltersRs
        (
            Glassware: [.. allCocktails
                .SelectMany(x => x.Glassware)
                .Select(x => Enum.Parse<GlasswareType>(x, true))
                .Where(x => x != GlasswareType.None)
                .Select(x => x.GetAttributeOfType<DisplayAttribute>())
                .Select(x => x?.Name.ToString() ?? string.Empty)
                .Distinct()
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .OrderBy(x => x)
                .Select(x => new IngredientFilterModel($"glassware-{x.ToLower().Replace(" ", "-")}", x))],
            Spirits: [.. ingredients
                .Where(x => x.Types.Contains(IngredientType.Spirit.ToString(), StringComparer.OrdinalIgnoreCase))
                .Select(x => new IngredientFilter(x.IngredientId, IngredientType.Spirit, x.BaseName))
                .DistinctBy(x => x.Name)
                .OrderBy(x => x.Name)
                .Select(x => new IngredientFilterModel2($"{x.Type.ToString().ToLower()}-{x.Id.ToLower()}", x.Name))],
            Liqueurs: [.. ingredients
                .Where(x => x.Types.Contains(IngredientType.Liqueur.ToString(), StringComparer.OrdinalIgnoreCase))
                .Select(x => new IngredientFilter(x.IngredientId, IngredientType.Liqueur, x.BaseName))
                .DistinctBy(x => x.Name)
                .OrderBy(x => x.Name)
                .Select(x => new IngredientFilterModel3($"{x.Type.ToString().ToLower()}-{x.Id.ToLower()}", x.Name))],
            Fruits: [.. ingredients
                .Where(x => x.Types.Contains(IngredientType.Fruit.ToString(), StringComparer.OrdinalIgnoreCase))
                .Select(x => new IngredientFilter(x.IngredientId, IngredientType.Fruit, x.BaseName))
                .DistinctBy(x => x.Name)
                .OrderBy(x => x.Name)
                .Select(x => new IngredientFilterModel4($"{x.Type.ToString().ToLower()}-{x.Id.ToLower()}", x.Name))],
            Vegetables: [.. ingredients
                .Where(x => x.Types.Contains(IngredientType.Vegetable.ToString(), StringComparer.OrdinalIgnoreCase))
                .Select(x => new IngredientFilter(x.IngredientId, IngredientType.Vegetable, x.BaseName))
                .DistinctBy(x => x.Name)
                .OrderBy(x => x.Name)
                .Select(x => new IngredientFilterModel5($"{x.Type.ToString().ToLower()}-{x.Id.ToLower()}", x.Name))],
            HerbsAndFlowers: [.. ingredients
                .Where(x => x.Types.Contains(IngredientType.Herb.ToString(), StringComparer.OrdinalIgnoreCase) || x.Types.Contains(IngredientType.Flowers.ToString(), StringComparer.OrdinalIgnoreCase))
                .Select(x => new IngredientFilter(x.IngredientId, x.Types.Contains(IngredientType.Herb.ToString(), StringComparer.OrdinalIgnoreCase) ? IngredientType.Herb : IngredientType.Flowers, x.BaseName))
                .DistinctBy(x => x.Name)
                .OrderBy(x => x.Name)
                .Select(x => new IngredientFilterModel6($"{x.Type.ToString().ToLower()}-{x.Id.ToLower()}", x.Name))],
            SyrupsAndSauces: [.. ingredients
                .Where(x => x.Types.Contains(IngredientType.Syrup.ToString(), StringComparer.OrdinalIgnoreCase) || x.Types.Contains(IngredientType.Sauce.ToString(), StringComparer.OrdinalIgnoreCase))
                .Select(x => new IngredientFilter(x.IngredientId, x.Types.Contains(IngredientType.Syrup.ToString(), StringComparer.OrdinalIgnoreCase) ? IngredientType.Syrup : IngredientType.Sauce, x.BaseName))
                .DistinctBy(x => x.Name)
                .OrderBy(x => x.Name)
                .Select(x => new IngredientFilterModel7($"{x.Type.ToString().ToLower()}-{x.Id.ToLower()}", x.Name))],
            Bitters: [.. ingredients
                .Where(x => x.Types.Contains(IngredientType.Bitters.ToString(), StringComparer.OrdinalIgnoreCase))
                .Select(x => new IngredientFilter(x.IngredientId, IngredientType.Bitters, x.BaseName))
                .DistinctBy(x => x.Name)
                .OrderBy(x => x.Name)
                .Select(x => new IngredientFilterModel8($"{x.Type.ToString().ToLower()}-{x.Id.ToLower()}", x.Name))],
            Proteins: [.. ingredients
                .Where(x => x.Types.Contains(IngredientType.Protein.ToString(), StringComparer.OrdinalIgnoreCase))
                .Select(x => new IngredientFilter(x.IngredientId, IngredientType.Protein, x.BaseName))
                .DistinctBy(x => x.Name)
                .OrderBy(x => x.Name)
                .Select(x => new IngredientFilterModel9($"{x.Type.ToString().ToLower()}-{x.Id.ToLower()}", x.Name))],
            Juices: [.. ingredients
                .Where(x => x.Types.Contains(IngredientType.Juice.ToString(), StringComparer.OrdinalIgnoreCase))
                .Select(x => new IngredientFilter(x.IngredientId, IngredientType.Juice, x.BaseName))
                .DistinctBy(x => x.Name)
                .OrderBy(x => x.Name)
                .Select(x => new IngredientFilterModel10($"{x.Type.ToString().ToLower()}-{x.Id.ToLower()}", x.Name))],
            Dilutions: [.. ingredients
                .Where(x => x.Types.Contains(IngredientType.Dilution.ToString(), StringComparer.OrdinalIgnoreCase))
                .Select(x => new IngredientFilter(x.IngredientId, IngredientType.Dilution, x.BaseName))
                .DistinctBy(x => x.Name)
                .OrderBy(x => x.Name)
                .Select(x => new IngredientFilterModel11($"{x.Type.ToString().ToLower()}-{x.Id.ToLower()}", x.Name))],
            BeerWineChampagne: [.. ingredients
                .Where(x => x.Types.Contains(IngredientType.Beer.ToString(), StringComparer.OrdinalIgnoreCase) || x.Types.Contains(IngredientType.Wine.ToString(), StringComparer.OrdinalIgnoreCase) || x.Types.Contains(IngredientType.Champagne.ToString(), StringComparer.OrdinalIgnoreCase))
                .Select(x => new IngredientFilter(x.IngredientId, x.Types.Contains(IngredientType.Beer.ToString(), StringComparer.OrdinalIgnoreCase) ? IngredientType.Beer : x.Types.Contains(IngredientType.Wine.ToString(), StringComparer.OrdinalIgnoreCase) ? IngredientType.Wine : IngredientType.Champagne, x.BaseName))
                .DistinctBy(x => x.Name)
                .OrderBy(x => x.Name)
                .Select(x => new IngredientFilterModel12($"{x.Type.ToString().ToLower()}-{x.Id.ToLower()}", x.Name))],
            Eras: [.. allCocktails
                .SelectMany(x => x.Eras)
                .Select(x => x)
                .Distinct()
                .OrderBy(x => x)
                .Select(x => new IngredientFilterModel13($"era-{x.ToLower().Replace(" ", "-")}", x))]
        );

        return Task.FromResult(filters);
    }

    public Task<CocktailIndexNowRs> GetCocktailsIndexNowResult(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult<CocktailIndexNowRs>(null);
        }

        var result = new CocktailIndexNowRs(
            UrlList: [.. cocktailRepository.CachedItems.Select(x => cocktailWebConfig.Value.SiteMap.CockailsPageFormat.Replace(":id", x.Id))]);

        return Task.FromResult(result);
    }

    private Task<List<Cocktail>> GetCocktails(
        string freeText,
        int skip = 0,
        int take = 20,
        bool allowExcessiveTake = false,
        List<string> filters = null,
        string[] matches = null,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult(new List<Cocktail>());
        }

        if (skip < 0)
        {
            skip = 0;
        }

        if (take is > 50 or <= 0)
        {
            if (take is > 50 && !allowExcessiveTake)
            {
                take = 50;
            }
            else if (take <= 0)
            {
                take = 50;
            }
        }

        var searchFilters = (filters ?? []).Where(x => x.IndexOf('-') > 0).ToList();
        var spiritFilters = searchFilters
            .Where(x => x.StartsWith($"{IngredientType.Spirit}-", true, CultureInfo.InvariantCulture))
            .Select(x => x.Replace($"{IngredientType.Spirit}-".ToLower(), string.Empty))
            .ToList();

        var liqueursFilters = searchFilters
            .Where(x => x.StartsWith($"{IngredientType.Liqueur}-", true, CultureInfo.InvariantCulture))
            .Select(x => x.Replace($"{IngredientType.Liqueur}-".ToLower(), string.Empty))
            .ToList();

        var fruitFilters = searchFilters
            .Where(x => x.StartsWith($"{IngredientType.Fruit}-", true, CultureInfo.InvariantCulture))
            .Select(x => x.Replace($"{IngredientType.Fruit}-".ToLower(), string.Empty))
            .ToList();

        var bittersFilters = searchFilters
            .Where(x => x.StartsWith($"{IngredientType.Bitters}-", true, CultureInfo.InvariantCulture))
            .Select(x => x.Replace($"{IngredientType.Bitters}-".ToLower(), string.Empty))
            .ToList();

        var vegiFilters = searchFilters
            .Where(x => x.StartsWith($"{IngredientType.Vegetable}-", true, CultureInfo.InvariantCulture))
            .Select(x => x.Replace($"{IngredientType.Vegetable}-".ToLower(), string.Empty))
            .ToList();

        var dilutionFilters = searchFilters
            .Where(x => x.StartsWith($"{IngredientType.Dilution}-", true, CultureInfo.InvariantCulture))
            .Select(x => x.Replace($"{IngredientType.Dilution}-".ToLower(), string.Empty))
            .ToList();

        var herbFilters = searchFilters
            .Where(x => x.StartsWith($"{IngredientType.Herb}-", true, CultureInfo.InvariantCulture))
            .Select(x => x.Replace($"{IngredientType.Herb}-".ToLower(), string.Empty))
            .ToList();

        var juiceFilters = searchFilters
            .Where(x => x.StartsWith($"{IngredientType.Juice}-", true, CultureInfo.InvariantCulture))
            .Select(x => x.Replace($"{IngredientType.Juice}-".ToLower(), string.Empty))
            .ToList();

        var proteinFilters = searchFilters
            .Where(x => x.StartsWith($"{IngredientType.Protein}-", true, CultureInfo.InvariantCulture))
            .Select(x => x.Replace($"{IngredientType.Protein}-".ToLower(), string.Empty))
            .ToList();

        var syrupsAndSaucesFilters = searchFilters
            .Where(x => x.StartsWith($"{IngredientType.Syrup}-", true, CultureInfo.InvariantCulture) || x.StartsWith($"{IngredientType.Sauce}-", true, CultureInfo.InvariantCulture))
            .Select(x => x.Replace($"{IngredientType.Syrup}-".ToLower(), string.Empty).Replace($"{IngredientType.Sauce}-".ToLower(), string.Empty))
            .ToList();

        var beersWinesAndChampagneFilters = searchFilters
            .Where(x => x.StartsWith($"{IngredientType.Beer}-", true, CultureInfo.InvariantCulture) || x.StartsWith($"{IngredientType.Wine}-", true, CultureInfo.InvariantCulture) || x.StartsWith($"{IngredientType.Champagne}-", true, CultureInfo.InvariantCulture))
            .Select(x => x.Replace($"{IngredientType.Beer}-".ToLower(), string.Empty).Replace($"{IngredientType.Wine}-".ToLower(), string.Empty).Replace($"{IngredientType.Champagne}-".ToLower(), string.Empty))
            .ToList();

        var glasswareFilters = searchFilters
            .Where(x => x.StartsWith("glassware-", true, CultureInfo.InvariantCulture))
            .Select(x => x.Replace("glassware-", string.Empty).Replace("-", " "))
            .ToList();

        var eraFilters = searchFilters
            .Where(x => x.StartsWith("era-", true, CultureInfo.InvariantCulture))
            .Select(x => x.Replace("era-", string.Empty).Replace("-", " "))
            .ToList();

        var filteredStart = cocktailRepository.CachedItems.AsEnumerable()
            .Where(x => matches == null || matches.Contains(x.Id))
            .Where(x => string.IsNullOrWhiteSpace(freeText) || x.SearchableTitles.Any(t => t.StartsWith(freeText, CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase)))
            .Where(x => ContainsIngriedentTypes(x, [IngredientType.Spirit], spiritFilters))
            .Where(x => ContainsIngriedentTypes(x, [IngredientType.Liqueur], liqueursFilters))
            .Where(x => ContainsIngriedentTypes(x, [IngredientType.Fruit], fruitFilters))
            .Where(x => ContainsIngriedentTypes(x, [IngredientType.Bitters], bittersFilters))
            .Where(x => ContainsIngriedentTypes(x, [IngredientType.Vegetable], vegiFilters))
            .Where(x => ContainsIngriedentTypes(x, [IngredientType.Dilution], dilutionFilters))
            .Where(x => ContainsIngriedentTypes(x, [IngredientType.Herb], herbFilters))
            .Where(x => ContainsIngriedentTypes(x, [IngredientType.Juice], juiceFilters))
            .Where(x => ContainsIngriedentTypes(x, [IngredientType.Protein], proteinFilters))
            .Where(x => ContainsIngriedentTypes(x, [IngredientType.Syrup, IngredientType.Sauce], syrupsAndSaucesFilters))
            .Where(x => ContainsIngriedentTypes(x, [IngredientType.Beer, IngredientType.Wine, IngredientType.Champagne], beersWinesAndChampagneFilters))
            .Where(x => ContainsGlasswareTypes(x, glasswareFilters))
            .Where(x => ContainsEraTypes(x, eraFilters))
            .Skip(skip)
            .Take(take)
            .ToList();

        var filteredContains = take - filteredStart.Count > 0
            ? cocktailRepository.CachedItems.AsEnumerable()
                .Where(x => matches == null || matches.Contains(x.Id))
                .Where(x => !filteredStart.Any(f => f.Id == x.Id))
                .Where(x => string.IsNullOrWhiteSpace(freeText) || x.SearchableTitles.Any(t => t.Contains(freeText, CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase)))
                .Where(x => ContainsIngriedentTypes(x, [IngredientType.Spirit], spiritFilters))
                .Where(x => ContainsIngriedentTypes(x, [IngredientType.Liqueur], liqueursFilters))
                .Where(x => ContainsIngriedentTypes(x, [IngredientType.Fruit], fruitFilters))
                .Where(x => ContainsIngriedentTypes(x, [IngredientType.Bitters], bittersFilters))
                .Where(x => ContainsIngriedentTypes(x, [IngredientType.Vegetable], vegiFilters))
                .Where(x => ContainsIngriedentTypes(x, [IngredientType.Dilution], dilutionFilters))
                .Where(x => ContainsIngriedentTypes(x, [IngredientType.Herb], herbFilters))
                .Where(x => ContainsIngriedentTypes(x, [IngredientType.Juice], juiceFilters))
                .Where(x => ContainsIngriedentTypes(x, [IngredientType.Protein], proteinFilters))
                .Where(x => ContainsIngriedentTypes(x, [IngredientType.Syrup, IngredientType.Sauce], syrupsAndSaucesFilters))
                .Where(x => ContainsIngriedentTypes(x, [IngredientType.Beer, IngredientType.Wine, IngredientType.Champagne], beersWinesAndChampagneFilters))
                .Where(x => ContainsGlasswareTypes(x, glasswareFilters))
                .Where(x => ContainsEraTypes(x, eraFilters))
                .Skip(skip)
                .Take(take - filteredStart.Count)
                .ToList()
            : [];

        return Task.FromResult(filteredStart.Concat(filteredContains).ToList());
    }

    private static bool ContainsIngriedentTypes(Cocktail item, List<IngredientType> types, List<string> searchFilters)
    {
        if (searchFilters.Count == 0)
        {
            return true;
        }

        return item.Ingredients
            .Where(x => x.Types.Any(t => types.Contains(Enum.Parse<IngredientType>(t, true))))
            .Where(x => searchFilters.Any(f => f.Equals(x.IngredientId, StringComparison.OrdinalIgnoreCase) || (!string.IsNullOrWhiteSpace(f) && f.Equals(x.ParentIngredientId, StringComparison.OrdinalIgnoreCase))))
            .Any();
    }

    private static bool ContainsGlasswareTypes(Cocktail item, List<string> searchFilters)
    {
        if (searchFilters.Count == 0)
        {
            return true;
        }

        return item.Glassware
            .Where(x => searchFilters.Any(f => GlasswareDisplayNameMapping.ContainsKey(f.ToLower()) && item.Glassware.Contains(GlasswareDisplayNameMapping[f.ToLower()].ToString(), StringComparer.OrdinalIgnoreCase)))
            .Any();
    }

    private static bool ContainsEraTypes(Cocktail item, List<string> searchFilters)
    {
        if (searchFilters.Count == 0)
        {
            return true;
        }

        return item.Eras
            .Where(x => searchFilters.Any(f => f.Equals(x, StringComparison.OrdinalIgnoreCase)))
            .Any();
    }
}

internal static class CocktailQueriesExtensions
{
    internal static bool StartsWith(this string str, string value, CultureInfo culture, CompareOptions options, int startIndex = 0)
    {
        if (str.Length - startIndex >= value.Length)
        {
            var ins = str.Substring(startIndex, value.Length);
            return string.Compare(ins, value, culture, options) == 0;
        }

        return false;
    }

    internal static bool Contains(this string str, string value, CultureInfo culture, CompareOptions options)
    {
        var ix = culture.CompareInfo.IndexOf(
            str,
            value,
            options); // returns 3

        return ix > 0;
    }
}
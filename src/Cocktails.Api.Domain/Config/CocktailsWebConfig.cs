namespace Cocktails.Api.Domain.Config;

public class CocktailsWebConfig
{
    public const string SectionName = "CocktailsWeb";

    public required SiteMapConfig SiteMap { get; set; }
}

namespace Cocktails.Api.Domain.Config;

public class CocktailsApiConfig
{
    public const string SectionName = "CocktailsApi";

    public string BaseImageUri { get; set; }

    public string BaseOpenApiUri { get; set; }

    public string ApimHostKey { get; set; }
}

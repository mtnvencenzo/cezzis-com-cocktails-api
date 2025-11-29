namespace Cocktails.Api.Domain.Config;

public class CocktailsApiConfig
{
    public const string SectionName = "CocktailsApi";

    public required string BaseImageUri { get; set; }

    public required string BaseOpenApiUri { get; set; }

    public required string ApimHostKey { get; set; }
}

namespace Cocktails.Api.Domain.Config;

public class CosmosDbConfig
{
    public const string SectionName = "CosmosDb";

    public string ConnectionString { get; set; }

    public string AccountEndpoint { get; set; }

    public required string DatabaseName { get; set; }

    public required string CocktailsContainerName { get; set; }

    public required string IngredientsContainerName { get; set; }

    public required string AccountsContainerName { get; set; }
}

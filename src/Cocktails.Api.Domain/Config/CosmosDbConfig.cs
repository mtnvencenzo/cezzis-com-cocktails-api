namespace Cocktails.Api.Domain.Config;

public class CosmosDbConfig
{
    public const string SectionName = "CosmosDb";

    public string ConnectionString { get; set; }

    public string AccountEndpoint { get; set; }

    public string DatabaseName { get; set; }

    public string CocktailsContainerName { get; set; }

    public string IngredientsContainerName { get; set; }

    public string AccountsContainerName { get; set; }
}

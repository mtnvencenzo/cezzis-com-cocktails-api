namespace Cocktails.Api.Domain.Config;

public class SearchConfig
{
    public const string SectionName = "Search";

    public required Uri Endpoint { get; set; }

    public required string IndexName { get; set; }

    public required string QueryKey { get; set; }

    public required bool UseSearchIndex { get; set; }
}

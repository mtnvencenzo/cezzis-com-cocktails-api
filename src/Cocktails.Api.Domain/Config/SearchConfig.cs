namespace Cocktails.Api.Domain.Config;

public class SearchConfig
{
    public const string SectionName = "Search";

    public Uri Endpoint { get; set; }

    public string IndexName { get; set; }

    public string QueryKey { get; set; }

    public bool UseSearchIndex { get; set; }
}

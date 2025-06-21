namespace Cocktails.Api.Domain.Config;

public class EmailHandlingConfig
{
    public const string SectionName = "EmailHandling";

    public string DefaultFromName { get; set; }
    public string DefaultFromAddress { get; set; }
    public string DefaultToName { get; set; }
    public string DefaultToAddress { get; set; }
}

namespace Cocktails.Api.Domain.Config;

public class Auth0Config
{
    public const string SectionName = "Auth0";

    public string Domain { get; set; }
    public string Audience { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}
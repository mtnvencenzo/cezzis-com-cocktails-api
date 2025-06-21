namespace Cocktails.Api.Domain.Config;

public class ScalarAuthorizationCodeFlow
{
    public string ClientId { get; set; }
    public List<string> Scopes { get; set; }
}

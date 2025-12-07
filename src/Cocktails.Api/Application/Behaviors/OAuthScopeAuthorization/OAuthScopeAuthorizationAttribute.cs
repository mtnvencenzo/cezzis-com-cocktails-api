namespace Cocktails.Api.Application.Behaviors.OAuthScopeAuthorization;

using Cocktails.Api.StartupExtensions;
using Microsoft.AspNetCore.Authorization;

public class OAuthScopeAuthorizationAttribute : AuthorizeAttribute, IAuthorizationRequirement, IAuthRequiredScopeMetadata
{
    public OAuthScopeAuthorizationAttribute(string scope)
    {
        this.Policy = $"scope:{scope}";
        this.AcceptedScope = [scope];
    }

    public IEnumerable<string> AcceptedScope { get; }
}
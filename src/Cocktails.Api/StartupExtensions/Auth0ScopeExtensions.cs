namespace Cocktails.Api.StartupExtensions;

using Cocktails.Api.Application.Behaviors.OAuthScopeAuthorization;

internal static class Auth0ScopeExtensions
{
    internal static RouteHandlerBuilder RequireScope(this RouteHandlerBuilder builder, string scope)
    {
        return builder.RequireAuthorization(new OAuthScopeAuthorizationAttribute(scope));
    }
}

public interface IAuthRequiredScopeMetadata
{
    IEnumerable<string> AcceptedScope { get; }
}


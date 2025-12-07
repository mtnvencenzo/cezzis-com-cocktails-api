namespace Cocktails.Api.Application.Behaviors.OAuthScopeAuthorization;

using Microsoft.AspNetCore.Authorization;

public class OAuthScopeAuthorizationHandler : AuthorizationHandler<OAuthScopeAuthorizationAttribute>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OAuthScopeAuthorizationAttribute requirement)
    {
        // Debug logging to understand what's happening
        var hasIdentity = context.User.Identity != null;
        var isAuthenticated = context.User.Identity?.IsAuthenticated == true;
        var identityType = context.User.Identity?.AuthenticationType;
        var claimCount = context.User.Claims.Count();

        if (context.User.Identity?.IsAuthenticated != true)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var scopes = context.User.FindFirst("scope")?.Value?.Split(' ') ??
            [];

        if (requirement.AcceptedScope.Any(requiredScope => scopes.Contains(requiredScope)))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}
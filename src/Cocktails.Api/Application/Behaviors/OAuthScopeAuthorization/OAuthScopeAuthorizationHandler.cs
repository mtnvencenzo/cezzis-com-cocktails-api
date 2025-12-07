namespace Cocktails.Api.Application.Behaviors.OAuthScopeAuthorization;

using Microsoft.AspNetCore.Authorization;

public class OAuthScopeAuthorizationHandler : AuthorizationHandler<OAuthScopeAuthorizationAttribute>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OAuthScopeAuthorizationAttribute requirement)
    {
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
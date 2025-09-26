namespace Cocktails.Api.StartupExtensions;

using Microsoft.AspNetCore.Authorization;

internal static class Auth0ScopeExtensions
{
    internal static RouteHandlerBuilder RequireScope(this RouteHandlerBuilder builder, string scope)
    {
        return builder.RequireAuthorization(new ScopeAuthorizationAttribute(scope));
    }
}

public class ScopeAuthorizationAttribute : AuthorizeAttribute, IAuthorizationRequirement, IAuthRequiredScopeMetadata
{
    public ScopeAuthorizationAttribute(string scope)
    {
        this.Policy = $"scope:{scope}";
        this.AcceptedScope = [scope];
    }

    public IEnumerable<string> AcceptedScope { get; }
}

public interface IAuthRequiredScopeMetadata
{
    IEnumerable<string> AcceptedScope { get; }
}

public class ScopeAuthorizationHandler : AuthorizationHandler<ScopeAuthorizationAttribute>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeAuthorizationAttribute requirement)
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
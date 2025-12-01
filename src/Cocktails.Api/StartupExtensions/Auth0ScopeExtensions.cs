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
        // Debug logging to understand what's happening
        var hasIdentity = context.User.Identity != null;
        var isAuthenticated = context.User.Identity?.IsAuthenticated == true;
        var identityType = context.User.Identity?.AuthenticationType;
        var claimCount = context.User.Claims.Count();

        // You can add logging here or set breakpoints to inspect these values
        // For now, let's log to console (remove this in production)
        Console.WriteLine($"Debug Auth: HasIdentity={hasIdentity}, IsAuthenticated={isAuthenticated}, AuthType='{identityType}', Claims={claimCount}");

        if (context.User.Identity?.IsAuthenticated != true)
        {
            Console.WriteLine("Authentication failed: User not authenticated");
            context.Fail();
            return Task.CompletedTask;
        }

        var scopes = context.User.FindFirst("scope")?.Value?.Split(' ') ??
            [];

        Console.WriteLine($"Found scopes: [{string.Join(", ", scopes)}], Required: [{string.Join(", ", requirement.AcceptedScope)}]");

        if (requirement.AcceptedScope.Any(requiredScope => scopes.Contains(requiredScope)))
        {
            Console.WriteLine("Authorization succeeded");
            context.Succeed(requirement);
        }
        else
        {
            Console.WriteLine("Authorization failed: Required scope not found");
            context.Fail();
        }

        return Task.CompletedTask;
    }
}
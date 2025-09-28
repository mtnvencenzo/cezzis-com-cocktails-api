namespace Cocktails.Api.StartupExtensions;

using Cocktails.Api.Application.Behaviors.ApimHostKeyAuthorization;
using Microsoft.AspNetCore.Authorization;

internal static class AuthorizationExtensions
{
    internal static IServiceCollection AddDefaultAuthorization(this IServiceCollection services)
    {
        // Adding apim host key authorization.  
        services.AddTransient<ApimHostKeyRequirementHandler>();
        services.AddTransient<IAuthorizationHandler, ApimHostKeyRequirementHandler>();

        // Adding Auth0 scope authorization
        services.AddTransient<ScopeAuthorizationHandler>();
        services.AddTransient<IAuthorizationHandler, ScopeAuthorizationHandler>();

        var authorizationBuilder = services.AddAuthorizationBuilder()
            .AddPolicy(ApimHostKeyRequirement.PolicyName, (o) =>
            {
                o.AddRequirements(new ApimHostKeyRequirement());
            });

        // Register scope-based policies - colons in names are perfectly fine
        RegisterScopePolicies(authorizationBuilder);

        return services;
    }

    private static void RegisterScopePolicies(AuthorizationBuilder authorizationBuilder)
    {
        // Define all the scopes used in your application
        var scopes = new[]
        {
            "read:owned-account",
            "write:owned-account"
        };

        foreach (var scope in scopes)
        {
            var policyName = $"scope:{scope}"; // Results in "scope:read:owned-account", etc.
            authorizationBuilder.AddPolicy(policyName, policy =>
            {
                policy.AddRequirements(new ScopeAuthorizationAttribute(scope));
            });
        }
    }
}

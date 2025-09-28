namespace Cocktails.Api.StartupExtensions;

using Cocktails.Api.Domain.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

internal static class AuthenticationExtensions
{
    internal static IServiceCollection AddDefaultAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var auth0Config = new Auth0Config();
        configuration.Bind(Auth0Config.SectionName, auth0Config);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                // Fix: Domain already includes https://, don't add it again
                options.Authority = auth0Config.Domain.TrimEnd('/') + "/";
                options.Audience = auth0Config.Audience;
                options.RequireHttpsMetadata = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = ClaimTypes.NameIdentifier,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromMinutes(5)
                    // Remove the custom IssuerSigningKeyResolver - let the middleware handle JWKS automatically
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        // Optional: Add custom claims processing here
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        // Add debugging for authentication failures
                        Console.WriteLine($"JWT Auth Failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }
}

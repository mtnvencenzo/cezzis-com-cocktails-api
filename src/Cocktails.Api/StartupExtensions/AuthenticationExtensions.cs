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
                options.Authority = $"https://{auth0Config.Domain}/";
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
                    ClockSkew = TimeSpan.FromMinutes(5),
                    IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                    {
                        // Fetch the JWKS from Auth0 to validate the token signature
                        var jwksUri = $"{auth0Config.Domain}.well-known/jwks.json";
                        var jwks = new JsonWebKeySet(new HttpClient().GetStringAsync(jwksUri).Result);
                        return jwks.Keys;
                    }
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        // Optional: Add custom claims processing here
                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }
}

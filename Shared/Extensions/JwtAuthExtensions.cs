using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Security;
using Microsoft.AspNetCore.Http;

namespace Shared.Extensions;

public static class JwtAuthExtensions
{
    public static void AddTrackPakAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        var auth0Namespace = configuration["Auth0:Namespace"] ?? "https://trackpak.dk";
        
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Auth0";
                options.DefaultChallengeScheme = "Auth0";
            })
            .AddJwtBearer("Auth0", options =>
            {
                options.Authority = "https://trackpak.eu.auth0.com/";
                options.Audience = "https://trackpak-prod.azurewebsites.net";
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = $"{auth0Namespace}/role" // Optional since you now use headers
                };
            });

        services.AddAuthorizationBuilder()
            // Permissions policies stay based on JWT claims
            .AddPolicy("RequireWriteReadEmployees", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c => c.Type == "permissions" &&
                        (c.Value.Contains("write:employees") || c.Value.Contains("read:employees") || c.Value.Contains("admin:all")))
                ))

            .AddPolicy("RequireWriteReadCompanies", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c => c.Type == "permissions" &&
                        (c.Value.Contains("write:companies") || c.Value.Contains("read:companies") || c.Value.Contains("admin:all")))
                ))

            // Role-based policies using HttpContext role extraction + hierarchy
            .AddPolicy(PolicyRoles.SystemAdmin, policy =>
                policy.RequireAssertion(context =>
                {
                    var httpContext = context.Resource as HttpContext;

                    var role = httpContext?.GetRole();
                    Console.WriteLine("Incoming roles: " + role);

                    return httpContext != null && httpContext.HasRole(RoleAsString.SystemAdmin);
                }))

            .AddPolicy(PolicyRoles.CompanyAdmin, policy =>
                policy.RequireAssertion(context =>
                {
                    var httpContext = context.Resource as HttpContext;
                    return httpContext != null && 
                        (httpContext.HasRole(RoleAsString.SystemAdmin) || httpContext.HasRole(RoleAsString.CompanyAdmin));
                }))

            .AddPolicy(PolicyRoles.Driver, policy =>
                policy.RequireAssertion(context =>
                {
                    var httpContext = context.Resource as HttpContext;
                    return httpContext != null &&
                        (httpContext.HasRole(RoleAsString.SystemAdmin) ||
                         httpContext.HasRole(RoleAsString.CompanyAdmin) ||
                         httpContext.HasRole(RoleAsString.Driver));
                }))

            .AddPolicy(PolicyRoles.Customer, policy =>
                policy.RequireAssertion(context =>
                {
                    var httpContext = context.Resource as HttpContext;
                    return httpContext != null && httpContext.HasRole(RoleAsString.Customer);
                }));
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Security;

namespace Shared.Extensions;

public static class JwtAuthExtensions
{ 
    public static void AddTrackPakAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        var auth0Namespace = "https://trackpak.dk";
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Auth0";
                options.DefaultChallengeScheme = "Auth0";
            })
            .AddJwtBearer("Auth0", options =>
            {
                options.Authority ="https://trackpak.eu.auth0.com/";
                options.Audience = "https://trackpak-prod.azurewebsites.net";
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = "permissions"
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireWritReadeEmployees", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c => c.Type == "permissions" && 
                                               (c.Value.Contains("write:employees")  ||  c.Value.Contains("read:employees") || c.Value.Contains("admin:all")))
                ));

            // System Admin has the "admin:all" scope OR systemAdmin role
            options.AddPolicy(PolicyRoles.SystemAdmin, policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c => c.Type == "scope" && c.Value.Contains("admin:all")) ||
                    context.User.HasClaim(c => c.Type == $"{auth0Namespace}/roles" && c.Value == RoleAsString.SystemAdmin)));

            options.AddPolicy(PolicyRoles.CompanyAdmin, policy =>
                policy.Requirements.Add(new RoleRequirement(new[] { RoleAsString.SystemAdmin, RoleAsString.CompanyAdmin })));

            options.AddPolicy(PolicyRoles.Driver, policy =>
                policy.Requirements.Add(new RoleRequirement(new[] { RoleAsString.SystemAdmin, RoleAsString.CompanyAdmin, RoleAsString.Driver })));

            options.AddPolicy(PolicyRoles.Customer, policy =>
                policy.Requirements.Add(new RoleRequirement(new[] { RoleAsString.Customer })));
        });
    }
}

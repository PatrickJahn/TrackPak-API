using System.Diagnostics;
using System.Text.Json;
using ApiGateway.Security.Roles;
using ApiGateway.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Monitoring;
using Ocelot.Authorization;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Shared.Middelware;
using Shared.Security;

var builder = WebApplication.CreateBuilder(args);

// Load Ocelot configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
// Add authentication with Auth0
builder.Services.AddAuthentication(o =>
    {
        o.DefaultAuthenticateScheme = "Auth0";
        o.DefaultChallengeScheme = "Auth0"; 
        
    })
    .AddJwtBearer("Auth0", options =>
    {
        options.Authority = builder.Configuration["Auth0:Authority"];
        options.Audience = builder.Configuration["Auth0:Audience"];
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
var auth0Namespace = builder.Configuration["Auth0:Namespace"];

builder.Services.AddAuthorization(options =>
{
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

builder.Services.AddSingleton<IClaimsAuthorizer, CustomPermissionsAuthorizer>();
builder.Services.AddSingleton<IAuthorizationHandler, RoleHandler>();


builder.Services.AddScoped<IUserContextService, UserContextService>(); 

// Add CORS service BEFORE using it
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Add Ocelot
builder.Services.AddOcelot(builder.Configuration).AddDelegatingHandler<CustomPermissionsAuthorizer>(); // ✅ Ensure Ocelot calls our custom authorizer;

var app = builder.Build();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<TracingMiddleware>();
app.UseMiddleware<OcelotHeaderMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Use(async (context, next) =>
{
    var user = context.User;
    if (user.Identity.IsAuthenticated)
    {
        Console.WriteLine("Authenticated User Claims:");
        foreach (var claim in user.Claims)
        {
            Console.WriteLine($"{claim.Type}: {claim.Value}");
        }
    }
    else
    {
        Console.WriteLine("User is NOT authenticated.");
    }
    await next();
});


app.UseWebSockets();

app.UseOcelot().Wait();

app.Run();

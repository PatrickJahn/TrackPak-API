using ApiGateway.Security.Roles;
using ApiGateway.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Shared.Middelware;

var builder = WebApplication.CreateBuilder(args);

// Load Ocelot configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
// Add authentication with Auth0
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            ClockSkew = TimeSpan.Zero
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


builder.Services.AddSingleton<IAuthorizationHandler, RoleHandler>();


builder.Services.AddScoped<IUserContextService, UserContextService>(); 

// Add CORS service BEFORE using it
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Add Ocelot
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.UseCors("AllowAll");

app.UseMiddleware<OcelotHeaderMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseWebSockets();

app.UseOcelot().Wait();

app.Run();

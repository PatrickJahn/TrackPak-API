using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    options.AddPolicy("Customer", policy =>
        policy.RequireClaim($"{auth0Namespace}/claims/roles", "Customer"));

    options.AddPolicy("CompanyAdmin", policy =>
        policy.RequireClaim($"{auth0Namespace}/claims/roles", "CompanyAdmin"));

    options.AddPolicy("Driver", policy =>
        policy.RequireClaim($"{auth0Namespace}/claims/roles", "Driver"));

    options.AddPolicy("SystemAdmin", policy =>
        policy.RequireClaim($"{auth0Namespace}/claims/roles", "SystemAdmin"));
});


// Add Ocelot
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseWebSockets();
app.UseOcelot().Wait();

app.Run();



using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using OrderService.Api.Endpoints;
using OrderService.Application;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure;
using Shared.Extensions;
using Shared.Middelware;
using Shared.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.Configure<GatewaySettings>(options =>
    options.AllowedGateways = builder.Configuration.GetSection("AllowedGateways").Get<string[]>() ?? Array.Empty<string>());

builder.Services.AddTrackPakAuthenticationAndAuthorization(builder.Configuration);
builder.Services.AddSingleton<IAuthorizationHandler, RoleHandler>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
//app.UseMiddleware<GatewayRestrictionMiddleware>();
app.UseMiddleware<TracingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.MapOrderEndpoints();
app.Run();
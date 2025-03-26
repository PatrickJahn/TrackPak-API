using EmployeeService.Api.Endpoints;
using EmployeeService.Application;
using EmployeeService.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Shared.Extensions;
using Shared.Middelware;
using Shared.Security;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddAuthentication();
builder.Services.AddTrackPakAuthenticationAndAuthorization(builder.Configuration);
builder.Services.AddSingleton<IAuthorizationHandler, RoleHandler>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<TracingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.MapEmployeeEndpoints();

app.Run();

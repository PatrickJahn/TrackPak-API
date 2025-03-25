using EmployeeService.Api.Endpoints;
using EmployeeService.Application;
using EmployeeService.Infrastructure;
using Shared.Middelware;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();


app.UseMiddleware<TracingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.MapEmployeeEndpoints();

app.Run();

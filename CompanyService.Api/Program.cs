using CompanyService.Api.Endpoints;
using CompanyService.Application;
using CompanyService.Infrastructure;
using Shared.Middelware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

//builder.Services.Configure<GatewaySettings>(options =>
  //  options.AllowedGateways = builder.Configuration.GetSection("AllowedGateways").Get<string[]>() ?? Array.Empty<string>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
//app.UseMiddleware<GatewayRestrictionMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.MapCompanyEndpoints();


app.Run();

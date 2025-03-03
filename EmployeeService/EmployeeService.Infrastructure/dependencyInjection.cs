using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Services;
using EmployeeService.Application.Repositories;
using EmployeeService.Infrastructure.DbContext;
using EmployeeService.Infrastructure.Repositories;

namespace EmployeeService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddDbContext<EmployeeDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));
       
        services.AddHttpClient<ILocationServiceClient, LocationServiceClient>(client =>
        {
            client.BaseAddress = new Uri("http://locationservice-api"); // Replace with actual URL
        });
        
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        // Ensure migrations are applied
        // Ensure migrations are applied in non-production environments
        using (var serviceProvider = services.BuildServiceProvider())
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
                var environment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

                if (environment.IsDevelopment())
                {
                    try
                    {
                        dbContext.Database.Migrate(); // Apply migrations
                        Console.WriteLine("Migrations applied successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error applying migrations: {ex.Message}");
                        throw;
                    }
                }
                else
                {
                    Console.WriteLine("Skipping migrations in production.");
                }
            }
        }

        return services;
    }
}

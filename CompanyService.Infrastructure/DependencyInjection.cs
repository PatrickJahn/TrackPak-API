using CompanyService.Domain.Interfaces;
using CompanyService.Infrastructure.DBContext;
using CompanyService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Services;

namespace CompanyService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddDbContext<CompanyDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));
       
       
        services.AddScoped<ICompanyRepository, CompanyRepository>();

        // Ensure migrations are applied
        // Ensure migrations are applied in non-production environments
        using (var serviceProvider = services.BuildServiceProvider())
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CompanyDbContext>();
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
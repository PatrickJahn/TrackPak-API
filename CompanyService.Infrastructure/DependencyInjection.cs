using CompanyService.Application.Interfaces;
using CompanyService.Domain.Interfaces;
using CompanyService.Infrastructure.DBContext;
using CompanyService.Infrastructure.Messaging;
using CompanyService.Infrastructure.Repositories;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Messaging;
using Shared.Services;

namespace CompanyService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddDbContext<CompanyDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));
       
        // ServiceBus config (RabbitMq)
        if (configuration.GetSection("ServiceBus").GetValue<bool>("useMock"))
        {
            services.AddSingleton<IMessageBus, MockRabbitMqServiceBus>();
        }
        else
        {
            services.AddEasyNetQ(configuration.GetConnectionString("RabbitMQ") ?? throw new InvalidOperationException());
            services.AddSingleton<IMessageBus, RabbitMqServiceBus>();
        }
        services.AddHostedService<MessageConsumerService>(); // Background listening service:))


        services.AddScoped<ICompanyEventPublisher, CompanyEventPublisher>();

        // Repositories
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
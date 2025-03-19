using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.DBContext;
using OrderService.Infrastructure.Messaging;
using OrderService.Infrastructure.Repositories;
using Shared.Messaging;

namespace OrderService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddDbContext<OrderDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));

        services.AddScoped<IOrderRepository, OrderRepository>();

           
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

        // Ensure migrations are applied
        // Ensure migrations are applied in non-production environments
        using (var serviceProvider = services.BuildServiceProvider())
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
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
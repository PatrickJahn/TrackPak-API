using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Messaging;
using Shared.Services;
using UserService.Application.Interfaces;
using UserService.Domain.Repositories;
using UserService.Infrastructure.DbContext;
using UserService.Infrastructure.Messaging;
using UserService.Infrastructure.Repositories;

namespace UserService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        
        
        // Database config (postgres)
        services.AddDbContext<UserDbContext>(options =>
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

        // Event Message publishers
        services.AddScoped<IUserEventPublisher, UserEventPublisher>();

        
        
        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        // Ensure migrations are applied
        using (var serviceProvider = services.BuildServiceProvider())
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
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
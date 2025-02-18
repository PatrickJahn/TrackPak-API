using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RouteService.Application.Interfaces;
using RouteService.Domain.Interfaces;
using RouteService.Infrastructure.DBContext;
using RouteService.Infrastructure.Grpc;
using RouteService.Infrastructure.Messaging;
using RouteService.Infrastructure.Repositories;
using RouteService.Infrastructure.Services;
using Shared.Messaging;

namespace RouteService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<RouteDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));
        
        var grpcChannel = GrpcChannel.ForAddress(configuration["Grpc:RouteOptimizer"]);
        services.AddSingleton(grpcChannel);
        services.AddScoped<GrpcRouteOptimizerClient>(); 
        services.AddScoped<IRouteOptimizer, RouteOptimizer>(); 

        services.AddScoped<IRouteRepository, RouteRepository>();
        services.AddScoped<IRouteService, RouteService.Application.Services.RouteService>();
        services.AddScoped<IRouteOptimizer, RouteOptimizer>(); // gRPC Optimization
       // services.AddScoped<IMessageBus, RabbitMqServiceBus>(); // RabbitMQ Messaging
      /// services.AddHostedService<RouteEventListener>(); // Event Listener Background Service
    

        // Ensure migrations are applied in non-production environments
        using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<RouteDbContext>();
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

        return services;
    }
}
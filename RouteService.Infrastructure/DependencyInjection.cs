using Microsoft.Extensions.DependencyInjection;
using RouteService.Application.Interfaces;
using RouteService.Domain.Interfaces;
using RouteService.Infrastructure.Messaging;
using RouteService.Infrastructure.Repositories;
using RouteService.Infrastructure.Services;
using Shared.Messaging;

public static class DependencyInjection
{
    public static IServiceCollection AddRouteService(this IServiceCollection services)
    {
        services.AddScoped<IRouteRepository, RouteRepository>();
        services.AddScoped<IRouteService, RouteService.Application.Services.RouteService>();
        services.AddScoped<IRouteOptimizer, RouteOptimizer>(); // gRPC Optimization
        services.AddScoped<IMessageBus, RabbitMqServiceBus>(); // RabbitMQ Messaging
        services.AddHostedService<RouteEventListener>(); // Event Listener Background Service

        return services;
    }
}
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Interfaces;
using OrderService.Application.Messaging.Handlers;
using Shared.Messaging;
using Shared.Messaging.Events.Location;
using Shared.Messaging.Events.User;

namespace OrderService.Application;

public static class DependencyInjection
{
    
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IOrderService, Services.OrderService>();
        
        // MessageHandlers
        services.AddScoped<IMessageHandler<OrderLocationCreatedEvent>, OrderLocationCreatedHandler>();
        services.AddScoped<IMessageHandler<OrderUserCreatedEvent>, OrderUserCreatedEventHandler>();

    }

}
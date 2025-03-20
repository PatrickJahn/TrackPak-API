using Microsoft.Extensions.DependencyInjection;
using Shared.Messaging;
using Shared.Messaging.Events.Location;
using Shared.Messaging.Events.Order;
using UserService.Application.Interfaces;
using UserService.Application.Messaging;
using UserService.Application.Messaging.Handlers;

namespace UserService.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, Services.UserService>();
        
        // MessageHandlers
        services.AddScoped<IMessageHandler<UserLocationCreatedEvent>, UserLocationCreatedHandler>();
        services.AddScoped<IMessageHandler<OrderCreatedEvent>, OrderCreatedHandler>();
        
        
    }
    

}
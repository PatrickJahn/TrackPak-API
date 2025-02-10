using LocationService.Application.Interfaces;
using LocationService.Application.Messaging;
using LocationService.Application.Messaging.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Shared.Messaging;
using Shared.Messaging.Events.Company;
using Shared.Messaging.Events.Employee;
using Shared.Messaging.Events.Order;
using Shared.Messaging.Events.User;

namespace LocationService.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ILocationEventPublisher, LocationEventPublisher>();
        services.AddHttpClient<IGeocodingService, Services.GeocodingService>();
        services.AddScoped<ILocationService, Services.LocationService>();
        
        // MessageHandlers
        services.AddScoped<IMessageHandler<UserCreatedEvent>, UserCreatedHandler>();
        services.AddScoped<IMessageHandler<CompanyCreatedEvent>, CompanyCreatedHandler>();
        services.AddScoped<IMessageHandler<OrderCreatedEvent>, OrderCreatedHandler>();
        services.AddScoped<IMessageHandler<EmployeeCreatedEvent>, EmployeeCreatedHandler>();

    }
}
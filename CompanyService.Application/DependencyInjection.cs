using CompanyService.Application.Interfaces;
using CompanyService.Application.Messaging.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Shared.Messaging;
using Shared.Messaging.Events.Location;

namespace CompanyService.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICompanyService, Services.CompanyService>();
        
           
        // MessageHandlers
        services.AddScoped<IMessageHandler<CompanyLocationCreatedEvent>, CompanyLocationCreatedHandler>();

    }
    
}
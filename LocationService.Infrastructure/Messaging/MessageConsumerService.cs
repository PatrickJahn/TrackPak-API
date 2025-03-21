using LocationService.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Messaging;
using Shared.Messaging.Events.Company;
using Shared.Messaging.Events.Employee;
using Shared.Messaging.Events.Order;
using Shared.Messaging.Events.User;
using Shared.Messaging.Topics;
using Shared.Models;

namespace LocationService.Infrastructure.Messaging;
public class MessageConsumerService(IMessageBus messageBus, IServiceProvider serviceProvider)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await messageBus.SubscribeAsync<UserCreatedEvent>(
            MessageTopic.UserCreated,  "LocationService",async (message) =>
            {
                using var scope = serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<UserCreatedEvent>>();
                await handler.HandleAsync(message, stoppingToken);
            });
        

        await messageBus.SubscribeAsync<OrderCreatedEvent>(
            MessageTopic.OrderCreated, "LocationService", async (message) =>
            {
                using var scope = serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<OrderCreatedEvent>>();
                await handler.HandleAsync(message, stoppingToken);
            });

        await messageBus.SubscribeAsync<EmployeeCreatedEvent>(
            MessageTopic.EmployeeCreated,  "LocationService", async (message) =>
            {
                using var scope = serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<EmployeeCreatedEvent>>();
                await handler.HandleAsync(message, stoppingToken);
            });

        await messageBus.SubscribeAsync<CompanyCreatedEvent>(
            MessageTopic.CompanyCreated, "LocationService", async (message) =>
            {
                using var scope = serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<CompanyCreatedEvent>>();
                await handler.HandleAsync(message, stoppingToken);
            });
        
        await messageBus.SubscribeAsync<UserLocationUpdatedEvent>(
            MessageTopic.UserLocationUpdated, "LocationService", async (message) =>
            {
                using var scope = serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<UserLocationUpdatedEvent>>();
                await handler.HandleAsync(message, stoppingToken);
            });
    }
}
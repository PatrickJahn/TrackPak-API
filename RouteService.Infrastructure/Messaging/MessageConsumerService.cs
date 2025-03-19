using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Messaging;
using Shared.Messaging.Events.Location;
using Shared.Messaging.Events.Order;
using Shared.Messaging.Events.Employee;
using Shared.Messaging.Topics;

namespace RouteService.Infrastructure.Messaging;

public class MessageConsumerService(IMessageBus messageBus, IServiceProvider serviceProvider) 
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Subscribe to Route-related events
        await messageBus.SubscribeAsync<OrderCreatedEvent>(MessageTopic.OrderCreated, OrderCreatedHandler);
        await messageBus.SubscribeAsync<OrderCancelledEvent>(MessageTopic.OrderCancelled, OrderCancelledHandler);
        await messageBus.SubscribeAsync<EmployeeCreatedEvent>(MessageTopic.EmployeeCreated, EmployeeCreatedHandler);
        await messageBus.SubscribeAsync<CompanyLocationCreatedEvent>(MessageTopic.CompanyLocationCreated, CompanyLocationCreatedHandler);

        // Event Handlers

        async void OrderCreatedHandler(OrderCreatedEvent message)
        {
            using var scope = serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<OrderCreatedEvent>>();

            try
            {
                // TODO: Fix
                Console.WriteLine($"OrderCreatedEvent received: OrderId: {message.OrderId}, CustomerId: {message}");
                await handler.HandleAsync(message, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling OrderCreatedEvent: {ex.Message}");
            }
        }

        async void OrderCancelledHandler(OrderCancelledEvent message)
        {
            using var scope = serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<OrderCancelledEvent>>();

            try
            {
                Console.WriteLine($"OrderCancelledEvent received: OrderId: {message.OrderId}");
                await handler.HandleAsync(message, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling OrderCancelledEvent: {ex.Message}");
            }
        }

        async void EmployeeCreatedHandler(EmployeeCreatedEvent message)
        {
            using var scope = serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<EmployeeCreatedEvent>>();

            try
            {
                Console.WriteLine($"EmployeeCreatedEvent received: EmployeeId: {message.EmployeeId}, CompanyId: {message.CompanyId}");
                await handler.HandleAsync(message, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling EmployeeCreatedEvent: {ex.Message}");
            }
        }

        async void CompanyLocationCreatedHandler(CompanyLocationCreatedEvent message)
        {
            using var scope = serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<CompanyLocationCreatedEvent>>();

            try
            {
                Console.WriteLine($"CompanyLocationCreatedEvent received: LocationId: {message.LocationId}, CompanyId: {message.CompanyId}");
                await handler.HandleAsync(message, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling CompanyLocationCreatedEvent: {ex.Message}");
            }
        }
    }
}

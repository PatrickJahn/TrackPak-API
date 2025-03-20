using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Messaging;
using Shared.Messaging.Events.Location;
using Shared.Messaging.Events.User;
using Shared.Messaging.Topics;

namespace OrderService.Infrastructure.Messaging;

public class MessageConsumerService(IMessageBus messageBus, IServiceProvider serviceProvider) 
    : BackgroundService
{
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      
        await messageBus.SubscribeAsync<OrderLocationCreatedEvent>(MessageTopic.OrderLocationCreated, OrderLocationCreatedHandler);
        
        await messageBus.SubscribeAsync<OrderUserCreatedEvent>(
            MessageTopic.OrderUserCreated, async (message) =>
            {
                using var scope = serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<OrderUserCreatedEvent>>();
                await handler.HandleAsync(message, stoppingToken);
            });

        
        
        async void OrderLocationCreatedHandler(OrderLocationCreatedEvent message)
        {
            using var scope = serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<OrderLocationCreatedEvent>>();

            try
            {
                Console.WriteLine($"OrderLocationCreatedEvent received: LocationId: {message.LocationId}, OrderId: {message.OrderId}");
                await handler.HandleAsync(message, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling message: {ex.Message}");
            }
        }

    }
}
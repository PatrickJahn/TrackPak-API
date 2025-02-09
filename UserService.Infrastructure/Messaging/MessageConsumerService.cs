using Microsoft.Extensions.Hosting;
using Shared.Messaging;
using Shared.Messaging.Events.Location;
using Shared.Messaging.Topics;

namespace UserService.Infrastructure.Messaging;

public class MessageConsumerService(
    IMessageBus messageBus, 
    IMessageHandler<UserLocationCreatedEvent> handler) : BackgroundService
{

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await messageBus.SubscribeAsync<UserLocationCreatedEvent>(MessageTopic.UserLocationCreated, async message =>
        {
            try
            {
                Console.WriteLine($"UserLocationCreatedEvent received: LocationId: {message.LocationId}, UserId: {message.UserId}");
                await handler.HandleAsync(message, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling message: {ex.Message}");
            }
        });
        
    }
}
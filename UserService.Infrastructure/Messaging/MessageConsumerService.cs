using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Shared.Messaging;
using Shared.Messaging.Events.Location;
using Shared.Messaging.Topics;

namespace UserService.Infrastructure.Messaging;

public class MessageConsumerService(IMessageBus messageBus, IServiceProvider serviceProvider) 
    : BackgroundService
{
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await messageBus.SubscribeAsync<UserLocationCreatedEvent>(MessageTopic.UserLocationCreated, async message =>
        {
            using var scope = serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<UserLocationCreatedEvent>>();

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
        await Task.Delay(Timeout.Infinite, stoppingToken);

    }
}
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Messaging;
using Shared.Messaging.Events.Location;
using Shared.Messaging.Topics;

namespace CompanyService.Infrastructure.Messaging;

public class MessageConsumerService(IMessageBus messageBus, IServiceProvider serviceProvider) 
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      
        await messageBus.SubscribeAsync<CompanyLocationCreatedEvent>(MessageTopic.CompanyLocationCreated, CompanyLocationCreatedHandler);
        
        
        async void CompanyLocationCreatedHandler(CompanyLocationCreatedEvent message)
        {
            using var scope = serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<CompanyLocationCreatedEvent>>();

            try
            {
                Console.WriteLine($"CompanyLocationCreatedEvent received: LocationId: {message.LocationId}, UserId: {message.CompanyId}");
                await handler.HandleAsync(message, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling message: {ex.Message}");
            }
        }

    }
}

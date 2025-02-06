using LocationService.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Messaging;
using Shared.Messaging.Events.User;
using Shared.Messaging.Topics;
using Shared.Models;

namespace LocationService.Infrastructure.Messaging;
public class MessageConsumerService : BackgroundService
{
    private readonly IMessageBus _messageBus;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public MessageConsumerService(IMessageBus messageBus, IServiceScopeFactory serviceScopeFactory)
    {
        _messageBus = messageBus;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _messageBus.SubscribeAsync<UserCreatedEvent>(
            MessageTopic.UserLocationCreated,
             (message) =>
            {
                 _ = HandleUserCreatedEvent(message);
            });
    }

    private async Task HandleUserCreatedEvent(UserCreatedEvent message)
    {
        using var scope = _serviceScopeFactory.CreateScope(); 
        var locationService = scope.ServiceProvider.GetRequiredService<ILocationService>(); 
        try
        {
            var locationId = await locationService.CreateLocationAsync(
                new CreateLocationRequestModel()
                {
                    City = message.Location.City,
                    AddressLine = message.Location.AddressLine,
                    PostalCode = message.Location.PostalCode,
                    Country = message.Location.Country
                });
            
            await _messageBus.PublishAsync(MessageTopic.UserLocationCreated, locationId);
        }
        catch (Exception e)
        {
            // TODO: Handle
            Console.WriteLine("Location service failed to create location or publish message");
            Console.WriteLine(e);
            throw;
        }
    }
    
}


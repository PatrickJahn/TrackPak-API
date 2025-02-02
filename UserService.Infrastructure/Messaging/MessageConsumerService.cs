using Microsoft.Extensions.Hosting;
using Shared.Messaging;
using Shared.Messaging.Messages;
using Shared.Messaging.Topics;

namespace UserService.Infrastructure.Messaging;

public class MessageConsumerService : BackgroundService
{
    private readonly IMessageBus _messageBus;

    public MessageConsumerService(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _messageBus.SubscribeAsync<UserLocationCreatedMessage>(MessageTopic.UserLocationCreated, message =>
        {
            
            // TODO: Call userService to update the users locationId - ooops add UserId to UserLocationCreatedMessage...
            Console.WriteLine($"[x] Received: {message.LocationId}");
        });
        
    }
}
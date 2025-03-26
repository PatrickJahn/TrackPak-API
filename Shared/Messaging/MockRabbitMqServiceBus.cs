using Monitoring;
using Shared.Messaging.Topics;
using Shared.Monitoring;

namespace Shared.Messaging;

public class MockRabbitMqServiceBus: IMessageBus
{
    
    public async Task PublishAsync<T>(MessageTopic topic, T message) where T : TracedMessage
    {
        Console.WriteLine($"Publishing message to topic {Enum.GetName(topic)}");
    }

    public async Task SubscribeAsync<T>(MessageTopic topic, string subscriptionId, Action<T> handler) where T : TracedMessage
    {
       Console.WriteLine($"Subscribing to topic {Enum.GetName(topic)}");
    }
}
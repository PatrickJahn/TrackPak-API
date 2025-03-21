using Shared.Messaging.Topics;

namespace Shared.Messaging;

public class MockRabbitMqServiceBus: IMessageBus
{
    
    public async Task PublishAsync<T>(MessageTopic topic, T message)
    {
        Console.WriteLine($"Publishing message to topic {Enum.GetName(topic)}");
    }

    public async Task SubscribeAsync<T>(MessageTopic topic, string subscriptionId, Action<T> handler)
    {
       Console.WriteLine($"Subscribing to topic {Enum.GetName(topic)}");
    }
}
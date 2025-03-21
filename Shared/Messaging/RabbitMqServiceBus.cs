using EasyNetQ;
using Shared.Messaging.Topics;

namespace Shared.Messaging;

public class RabbitMqServiceBus(IBus bus) : IMessageBus
{
    
    private readonly IBus _bus = bus ?? throw new ArgumentNullException(nameof(bus));

    public async Task PublishAsync<T>(MessageTopic topic, T message)
    {
        //TODO: Add retry or circuit breaker logic 
        Console.WriteLine("Publishing message to RabbitMQ, Topic: " + Enum.GetName(topic));
       await _bus.PubSub.PublishAsync(message, Enum.GetName(topic));
    }

    public async Task SubscribeAsync<T>(MessageTopic topic, string subscriberId, Action<T> handler)
    {
        var subscriptionId = $"{Enum.GetName(topic)}-{subscriberId}";
        Console.WriteLine("Subscription to RabbitMQ, Topic: " + subscriptionId);

        await _bus.PubSub.SubscribeAsync<T>(subscriptionId, async message =>
        {
            Console.WriteLine("Received message: " + subscriptionId);
            handler(message);
        });
    }
}
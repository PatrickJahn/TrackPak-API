using EasyNetQ;
using Shared.Messaging.Topics;

namespace Shared.Messaging;

public class RabbitMqServiceBus : IMessageBus
{
    
    private readonly IBus _bus;

    public RabbitMqServiceBus(IBus bus)
    {
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
    }
    
    public async Task PublishAsync<T>(MessageTopic topic, T message)
    {
        //TODO: Add retry or circuit breaker logic 
       await _bus.PubSub.PublishAsync(Enum.GetName(topic), nameof(topic));
    }

    public async Task SubscribeAsync<T>(MessageTopic topic, Action<T> handler)
    {
       await _bus.PubSub.SubscribeAsync(Enum.GetName(topic)!, handler);
    }
}
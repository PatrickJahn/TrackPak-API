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
        Console.WriteLine("Publishing message to RabbitMQ, Topic: " + Enum.GetName(topic));
       await _bus.PubSub.PublishAsync(message, Enum.GetName(topic));
    }

    public async Task SubscribeAsync<T>(MessageTopic topic, Action<T> handler)
    {
        Console.WriteLine("Subscription to RabbitMQ, Topic: " + Enum.GetName(topic));

       await _bus.PubSub.SubscribeAsync<T>(Enum.GetName(topic)!,  async message =>
       {
           Console.WriteLine("Received message: " + Enum.GetName(topic));
           handler(message);
       });
    }
}
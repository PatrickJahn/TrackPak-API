using Shared.Messaging.Topics;

namespace Shared.Messaging;

public interface IMessageBus
{
    Task PublishAsync<T>(MessageTopic topic, T message);
    Task SubscribeAsync<T>(MessageTopic topic, string subscriberId, Action<T> handler);
}
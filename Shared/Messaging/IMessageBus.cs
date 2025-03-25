using Monitoring;
using Shared.Messaging.Topics;
using Shared.Monitoring;

namespace Shared.Messaging;

public interface IMessageBus
{
    Task PublishAsync<T>(MessageTopic topic, T message) where T : TracedMessage;
    Task SubscribeAsync<T>(MessageTopic topic, string subscriberId, Action<T> handler) where T : TracedMessage;
}
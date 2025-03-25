using Monitoring;
using Shared.Monitoring;

namespace Shared.Messaging.Events.User;

public class OrderUserCreatedEvent : TracedMessage
{
    
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }

}
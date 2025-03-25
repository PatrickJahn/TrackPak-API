using Monitoring;

namespace Shared.Messaging.Events.Location;

public class OrderLocationCreatedEvent : TracedMessage
{
    
    public Guid OrderId { get; set; }
    public Guid LocationId { get; set; }
}
namespace Shared.Messaging.Events.Location;

public class OrderLocationCreatedEvent
{
    
    public Guid OrderId { get; set; }
    public Guid LocationId { get; set; }
}
namespace Shared.Messaging.Events.User;

public class OrderUserCreatedEvent
{
    
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }

}
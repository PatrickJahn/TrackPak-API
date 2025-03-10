namespace Shared.Messaging.Events.Location;

public class UserLocationCreatedEvent
{
    public Guid UserId { get; set; }
    public Guid LocationId { get; set; }
}
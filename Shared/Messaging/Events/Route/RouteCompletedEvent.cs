namespace Shared.Messaging.Events.Route;

public class RouteCompletedEvent
{
    public Guid RouteId { get; set; }
    public DateTime CompletedAt { get; set; }
}
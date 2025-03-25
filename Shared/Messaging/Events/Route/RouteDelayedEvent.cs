using Monitoring;

namespace Shared.Messaging.Events.Route;

public class RouteDelayedEvent : TracedMessage
{
    public Guid RouteId { get; set; }
    public TimeSpan DelayDuration { get; set; }
    public string Reason { get; set; } = string.Empty;
}
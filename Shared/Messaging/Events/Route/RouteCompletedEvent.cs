using Monitoring;
using Shared.Monitoring;

namespace Shared.Messaging.Events.Route;

public class RouteCompletedEvent : TracedMessage
{
    public Guid RouteId { get; set; }
    public DateTime CompletedAt { get; set; }
}
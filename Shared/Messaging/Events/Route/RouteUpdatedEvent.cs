using Monitoring;
using Shared.Enums;

namespace Shared.Messaging.Events.Route;

public class RouteUpdatedEvent : TracedMessage
{
    public Guid RouteId { get; set; }
    public RouteStatusEnum Status { get; set; }
    public DateTime? EstimatedCompletionTime { get; set; }
}
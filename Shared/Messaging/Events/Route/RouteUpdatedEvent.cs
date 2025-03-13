using Shared.Enums;

namespace Shared.Messaging.Events.Route;

public class RouteUpdatedEvent
{
    public Guid RouteId { get; set; }
    public RouteStatusEnum Status { get; set; }
    public DateTime? EstimatedCompletionTime { get; set; }
}
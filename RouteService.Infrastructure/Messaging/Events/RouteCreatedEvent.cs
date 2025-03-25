using Monitoring;
using RouteService.Domain.Entities;
using Shared.Monitoring;

namespace RouteService.Infrastructure.Messaging.Events;

public class RouteCreatedEvent: TracedMessage
{
    public Guid RouteId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? EmployeeId { get; set; }
    public List<OrderRoute> OrderRoutes { get; set; } = new();
    public DateTime? EstimatedCompletionTime { get; set; }
}
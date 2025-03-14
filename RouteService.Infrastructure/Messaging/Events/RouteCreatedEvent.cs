using RouteService.Domain.Entities;

namespace RouteService.Infrastructure.Messaging.Events;

public class RouteCreatedEvent
{
    public Guid RouteId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? EmployeeId { get; set; }
    public List<OrderRoute> OrderRoutes { get; set; } = new();
    public DateTime? EstimatedCompletionTime { get; set; }
}
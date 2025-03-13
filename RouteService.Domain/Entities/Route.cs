using Shared.Enums;
using Shared.Models;

namespace RouteService.Domain.Entities;

public class Route : BaseModel
{
    public Guid? EmployeeId { get; set; }
    public Guid CompanyId { get; set; }
    public List<OrderRoute> OrderRoutes { get; set; } = new(); // ✅ Use List<OrderRoute> instead of Dictionary<int, Guid>
    public RouteStatusEnum Status { get; set; }
    public DateTime? CompletedAt { get; set; }
}
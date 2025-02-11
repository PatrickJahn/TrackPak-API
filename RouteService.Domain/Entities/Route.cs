using RouteService.Domain.Enums;
using Shared.Models;

namespace RouteService.Domain.Entities;

public class Route : BaseModel
{
    public Guid EmployeeId { get; set; }
    public Guid CompanyId { get; set; }
    public Dictionary<int, Guid> OrderIds { get; set; }
    public RouteStatusEnum Status { get; set; }
}
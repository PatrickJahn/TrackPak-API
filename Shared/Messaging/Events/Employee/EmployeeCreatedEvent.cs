using Shared.Models;

namespace Shared.Messaging.Events.Employee;

public class EmployeeCreatedEvent
{
    public Guid EmployeeId { get; set; }
    public Guid CompanyId { get; set; }
    public CreateLocationRequestModel Location { get; set;  }
}
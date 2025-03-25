using Monitoring;
using Shared.Models;
using Shared.Monitoring;

namespace Shared.Messaging.Events.Employee;

public class EmployeeCreatedEvent  : TracedMessage
{
    public Guid EmployeeId { get; set; }
    public Guid CompanyId { get; set; }
    public CreateLocationRequestModel Location { get; set;  }
}
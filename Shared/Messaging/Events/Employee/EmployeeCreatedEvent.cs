using Shared.Models;

namespace Shared.Messaging.Events.Employee;

public class EmployeeCreatedEvent
{
    public Guid EmployeeId { get; }
    public CreateLocationRequestModel Location { get; }
}
using Monitoring;
using Shared.Monitoring;

namespace Shared.Messaging.Events.Employee;

public class EmployeeCheckedOutEvent  : TracedMessage
{
    public Guid EmployeeId { get; set; } = Guid.Empty;
    public DateTime CheckedOutAt { get; set; } = DateTime.UtcNow;
}
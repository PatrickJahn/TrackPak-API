using Monitoring;

namespace Shared.Messaging.Events.Employee;

public class EmployeeCheckedInEvent  : TracedMessage
{
    public Guid EmployeeId { get; set; } = Guid.Empty;
    public DateTime CheckedInAt { get; set; } = DateTime.UtcNow;
}
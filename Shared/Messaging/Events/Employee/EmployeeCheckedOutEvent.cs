namespace Shared.Messaging.Events.Employee;

public class EmployeeCheckedOutEvent
{
    public Guid EmployeeId { get; set; } = Guid.Empty;
    public DateTime CheckedOutAt { get; set; } = DateTime.UtcNow;
}
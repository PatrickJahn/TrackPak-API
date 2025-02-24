namespace Shared.Messaging.Events.Location;

public class EmployeeLocationCreatedEvent
{
    public Guid EmployeeId { get; set; }
    public Guid LocationId { get; set; }
}
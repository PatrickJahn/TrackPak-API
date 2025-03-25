using Monitoring;

namespace Shared.Messaging.Events.Location;

public class EmployeeLocationCreatedEvent : TracedMessage
{
    public Guid EmployeeId { get; set; }
    public Guid LocationId { get; set; }
}
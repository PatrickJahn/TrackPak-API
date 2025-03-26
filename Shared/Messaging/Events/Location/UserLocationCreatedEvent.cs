using Monitoring;
using Shared.Monitoring;

namespace Shared.Messaging.Events.Location;

public class UserLocationCreatedEvent : TracedMessage
{
    public Guid UserId { get; set; }
    public Guid LocationId { get; set; }
}
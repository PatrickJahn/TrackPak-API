using Monitoring;

namespace Shared.Messaging.Events.Location;

public class CompanyLocationCreatedEvent  : TracedMessage
{
    public Guid CompanyId { get; set; }
    public Guid LocationId { get; set; }
}
using Monitoring;
using Shared.Monitoring;

namespace Shared.Messaging.Events.Order;

public class CompanyOrdersFetchedEvent : TracedMessage
{
    public Guid CompanyId { get; set; }
    public List<Guid> OrderIds { get; set; } = [];
}
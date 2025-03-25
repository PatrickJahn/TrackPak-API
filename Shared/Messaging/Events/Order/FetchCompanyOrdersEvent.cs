using Monitoring;
using Shared.Monitoring;

namespace Shared.Messaging.Events.Order;

public class FetchCompanyOrdersEvent : TracedMessage
{
    public Guid CompanyId { get; set; }
}

using Monitoring;
using Shared.Models;
using Shared.Monitoring;

namespace Shared.Messaging.Events.Company;

public class CompanyCreatedEvent : TracedMessage
{
    public Guid CompanyId { get; set;  }
    public CreateLocationRequestModel Location { get; set; }
}
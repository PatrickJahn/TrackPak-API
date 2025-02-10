using Shared.Models;

namespace Shared.Messaging.Events.Company;

public class CompanyCreatedEvent
{
    public Guid CompanyId { get; set;  }
    public CreateLocationRequestModel Location { get; set; }
}
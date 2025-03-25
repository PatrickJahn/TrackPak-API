using Monitoring;
using Shared.Models;
using Shared.Monitoring;

namespace Shared.Messaging.Events.User;

public class UserLocationUpdatedEvent : TracedMessage
{
    public Guid UserId { get; set; }
 
    public CreateLocationRequestModel Location { get; set; }
}
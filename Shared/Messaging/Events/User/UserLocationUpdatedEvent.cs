using Monitoring;
using Shared.Models;

namespace Shared.Messaging.Events.User;

public class UserLocationUpdatedEvent : TracedMessage
{
    public Guid UserId { get; set; }
 
    public CreateLocationRequestModel Location { get; set; }
}
using Monitoring;
using Shared.Models;
using Shared.Monitoring;

namespace Shared.Messaging.Events.Order;

public class OrderCreatedEvent : TracedMessage
{
    public Guid OrderId { get; set; }
    
    public CreateUserRequestModel? User { get; set; }
    public CreateLocationRequestModel Location { get; set; }
}
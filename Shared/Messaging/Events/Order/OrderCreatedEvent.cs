using Shared.Models;

namespace Shared.Messaging.Events.Order;

public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    
    public CreateUserRequestModel? User { get; set; }
    public CreateLocationRequestModel Location { get; set; }
}
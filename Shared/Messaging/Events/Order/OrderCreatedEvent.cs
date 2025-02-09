using Shared.Models;

namespace Shared.Messaging.Events.Order;

public class OrderCreatedEvent
{
    public Guid OrderId { get; }
    public Guid? UserId { get; }
    public CreateLocationRequestModel Location { get; }
}
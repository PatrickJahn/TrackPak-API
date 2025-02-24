using Shared.Models;

namespace Shared.Messaging.Events.Order;

public class OrderUpdatedEvent
{
    public Guid OrderId { get; set; }
    public OrderStatusRequest Status { get; set; } // e.g., "In Progress", "Completed"
    public Guid UpdatedBy { get; set; }
    public DateTime Timestamp { get; set; }
}

using Shared.Models;

namespace Shared.Messaging.Events.Order;

public class OrderUpdatedEvent
{
    public string OrderId { get; set; }
    public OrderStatusRequest Status { get; set; } // e.g., "In Progress", "Completed"
    public string UpdatedBy { get; set; }
    public DateTime Timestamp { get; set; }
}

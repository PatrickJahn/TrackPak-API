using Shared.Models;

namespace Shared.Messaging.Events.Order;

public class OrderCancelledEvent
{
    public Guid OrderId { get; set; }  
    public Guid CustomerId { get; set; }  // ID of the customer who placed the order
    public Guid CancelledBy { get; set; }  // Indicates if cancelled by "Customer", "Admin", or "System"
    public string Reason { get; set; }  // Explanation of cancellation (e.g., "Customer request", "Stock unavailable")
    public DateTime CancelledAt { get; set; }  // Timestamp of when the cancellation occurred
    public OrderStatusRequest? Status {get; set;} = OrderStatusRequest.Cancelled; // Order Status set to Cancelled by default
}

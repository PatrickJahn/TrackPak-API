using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
namespace OrderService.Application.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(Order order, CancellationToken cancellationToken = default);
        Task<Order?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Order>> GetOrdersAsync(
            Guid? userId, 
            Guid? companyId, 
            OrderStatus? status, 
            CancellationToken cancellationToken = default);
        Task<bool> UpdateOrderAsync(Guid orderId, Order updatedOrder, CancellationToken cancellationToken = default);
        Task<bool> DeleteOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus, CancellationToken cancellationToken = default);
        Task<bool> CancelOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    }
}
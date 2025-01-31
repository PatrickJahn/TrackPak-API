using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Domain.Interfaces;


namespace OrderService.Application.Services
{
    public class OrderService(IOrderRepository orderRepository) : IOrderService
    {
        public async Task<Order> CreateOrderAsync(Order order, CancellationToken cancellationToken = default)
        {
            order.Id = Guid.NewGuid();
            order.Status = OrderStatus.Pending;
            order.CreatedAt = DateTime.UtcNow;

            await orderRepository.AddAsync(order);

            // Publish OrderCreatedEvent via messaging

            return order;
        }

        public async Task<Order?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await orderRepository.GetByIdAsync(orderId);
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(
            Guid? userId, 
            Guid? companyId, 
            OrderStatus? status, 
            CancellationToken cancellationToken = default)
        {
            return await orderRepository.GetOrdersAsync(userId, companyId, status, cancellationToken);
        }

        public async Task<bool> UpdateOrderAsync(Guid orderId, Order updatedOrder, CancellationToken cancellationToken = default)
        {
            var existingOrder = await orderRepository.GetByIdAsync(orderId);
            if (existingOrder == null) return false;

            existingOrder.Description = updatedOrder.Description;
            existingOrder.OrderItems = updatedOrder.OrderItems;
            existingOrder.LastModifiedAt = DateTime.UtcNow;

            await orderRepository.Update(existingOrder);

            // Publish OrderUpdatedEvent via messaging

            return true;
        }

        public async Task<bool> DeleteOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var orderExists = await orderRepository.ExistsAsync(orderId, cancellationToken);
            if (!orderExists) return false;

            await orderRepository.DeleteByIdAsync(orderId);

            // Publish OrderDeletedEvent via messaging

            return true;
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus, CancellationToken cancellationToken = default)
        {
            var updated = await orderRepository.UpdateOrderStatusAsync(orderId, newStatus, cancellationToken);

            // Publish OrderStatusUpdatedEvent via messaging

            return updated;
        }

        public async Task<bool> CancelOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await UpdateOrderStatusAsync(orderId, OrderStatus.Cancelled, cancellationToken);
        }
    }
}

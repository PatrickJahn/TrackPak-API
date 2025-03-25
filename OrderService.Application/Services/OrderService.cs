using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Monitoring;
using OrderService.Application.Interfaces;
using OrderService.Application.Models;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Domain.Interfaces;


namespace OrderService.Application.Services
{
    public class OrderService(IOrderRepository orderRepository, IOrderEventPublisher eventPublisher) : IOrderService
    {
        public async Task<Order> CreateOrderAsync(CreateOrderModel order, CancellationToken cancellationToken = default)
        {
     
            
            using var activity = LoggingService.activitySource.StartActivity("CreateOrder", ActivityKind.Internal);          
            activity?.SetTag("company.id", order.CompanyId);
            
          var newOrder = new Order
          {
              Type = order.Type,
              CompanyId = order.CompanyId,
              Description = order.Description,
              OrderItems = order.OrderItems
                  .Select(x => new OrderItem
                  {
                      Title = x.Title,
                      Price = x.Price,
                      Quantity = x.Quantity,
                  }).ToList(),
          };

            await orderRepository.AddAsync(newOrder);

            // Publish OrderCreatedEvent via messaging
            await eventPublisher.PublishOrderCreatedAsync(newOrder, order.Location, order.User);

            return newOrder;
        }

        public async Task<Order?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            using var activity = LoggingService.activitySource.StartActivity("GetOrderById", ActivityKind.Internal);
            activity?.SetTag("order.id", orderId);
            
            return await orderRepository.GetOrDefaultByIdAsync(orderId, (i) => i.Include(o => o.OrderItems));
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(
            Guid? userId, 
            Guid? companyId, 
            OrderStatus? status, 
            CancellationToken cancellationToken = default)
        {
                        
            using var activity = LoggingService.activitySource.StartActivity("GetOrdersAsync", ActivityKind.Internal);

            return await orderRepository.GetOrdersAsync(userId, companyId, status, cancellationToken);
        }

        public async Task<bool> UpdateOrderAsync(Guid orderId, Order updatedOrder, CancellationToken cancellationToken = default)
        {
            using var activity = LoggingService.activitySource.StartActivity("UpdateOrder", ActivityKind.Internal);
            activity?.SetTag("order.id", orderId);
            
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

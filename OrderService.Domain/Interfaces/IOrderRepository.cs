using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService.Domain.Interfaces
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        /// <summary>
        /// Retrieves orders with optional filters for user, company, and status.
        /// </summary>
        Task<IEnumerable<Order>> GetOrdersAsync(
            Guid? userId, 
            Guid? companyId, 
            OrderStatus? status, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the status of an existing order.
        /// </summary>
        Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if an order exists by ID.
        /// </summary>
        Task<bool> ExistsAsync(Guid orderId, CancellationToken cancellationToken = default);
    }
}
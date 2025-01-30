using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.DBContext;
using Shared.Repositories;

namespace OrderService.Infrastructure.Repositories
{
    public class OrderRepository(OrderDbContext dbContext) 
        : BaseRepository<Order, OrderDbContext>(dbContext), IOrderRepository
    {
        public async Task<IEnumerable<Order>> GetOrdersAsync(
            Guid? userId, 
            Guid? companyId, 
            OrderStatus? status, 
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Orders.AsQueryable();

            if (userId.HasValue)
                query = query.Where(o => o.UserId == userId.Value);

            if (companyId.HasValue)
                query = query.Where(o => o.CompanyId == companyId.Value);

            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            return await query.Include(o => o.OrderItems).ToListAsync(cancellationToken);
        }

        public async Task<bool> UpdateOrderStatusAsync(
            Guid orderId, 
            OrderStatus newStatus, 
            CancellationToken cancellationToken = default)
        {
            var order = await _dbContext.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.Status = newStatus;
            order.LastModifiedAt = DateTime.UtcNow;

            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> ExistsAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Orders.AnyAsync(o => o.Id == orderId, cancellationToken);
        }
    }
}
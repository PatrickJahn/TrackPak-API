using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.DBContext;
using Shared.Repositories;

namespace OrderService.Infrastructure.Repositories;

public class OrderRepository(OrderDbContext dbContext) : BaseRepository<Order, OrderDbContext>(dbContext), IOrderRepository
{
    
}
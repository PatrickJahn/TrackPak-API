using OrderService.Domain.Entities;
using Shared.Interfaces;

namespace OrderService.Domain.Interfaces;

public interface IOrderRepository : IBaseRepository<Order>
{
    
}
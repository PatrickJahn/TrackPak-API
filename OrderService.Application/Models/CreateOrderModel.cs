using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using Shared.Models;

namespace OrderService.Application.Models;

public class CreateOrderModel
{
    public Guid CompanyId { get; set; }
    
    public CreateUserRequestModel? User { get; set; }

    public CreateLocationRequestModel Location { get; set; }

    public OrderType Type { get; set; }
    public string Description { get; set; }
    
    public List<OrderItemDto?> OrderItems { get; set; } = new();
}


public class OrderItemDto 
{ 
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string Title { get; set; }
}
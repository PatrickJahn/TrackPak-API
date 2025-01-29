using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models;

namespace OrderService.Domain.Entities
{
    [Table("OrderItems")]

    public class OrderItem : BaseModel
    { 
        public Guid OrderId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Title { get; set; }
        public Order Order { get; set; }
    }
}
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Shared.Models;

namespace OrderService.Domain.Entities
{
    [Table("OrderItems")]

    public class OrderItem : BaseModel
    { 
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Title { get; set; }
        
        
        [JsonIgnore]
        public Order Order { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using OrderService.Domain.Enums;
using Shared.Models;

namespace OrderService.Domain.Entities
{
    [Table("Orders")]

    public class Order : BaseModel
    {
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public Guid LocationId { get; set; }

        public OrderStatus Status { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
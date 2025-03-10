using System;

namespace RouteService.Domain.Entities;

public class OrderRoute
{
    public int Sequence { get; set; } 
    public Guid OrderId { get; set; } 
    public Guid RouteId { get; set; } // Foreign key to Route
    public Route Route { get; set; }  // Navigation property
}
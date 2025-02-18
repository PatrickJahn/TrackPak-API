using RouteService.Domain.Entities;
using Shared.Messaging.Events.Order;

namespace RouteService.Application.Interfaces;

public interface IRouteService
{
    Task HandleOrderCreatedAsync(OrderCreatedEvent orderEvent);
    Task HandleOrderCancelledAsync(OrderCancelledEvent orderEvent);
    Task HandleEmployeeCheckedInAsync(EmployeeCheckedInEvent employeeEvent);
    Task HandleEmployeeCheckedOutAsync(EmployeeCheckedOutEvent employeeEvent);
    Task<List<Route>> GetRoutesByEmployeeIdAsync(string employeeId);
    Task<Route> MarkRouteAsCompletedAsync(string routeId);
    Task OptimizeRouteAsync(string routeId);
}
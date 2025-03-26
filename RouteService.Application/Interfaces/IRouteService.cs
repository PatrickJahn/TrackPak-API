using RouteService.Domain.Entities;
using Shared.Messaging.Events.Employee;
using Shared.Messaging.Events.Order;

namespace RouteService.Application.Interfaces
{
    public interface IRouteService
    {
        Task HandleOrderCreatedAsync(OrderCreatedEvent orderEvent);
        Task HandleOrderCancelledAsync(OrderCancelledEvent orderEvent);
        Task HandleEmployeeCheckedInAsync(EmployeeCheckedInEvent employeeEvent);
        Task HandleEmployeeCheckedOutAsync(EmployeeCheckedOutEvent employeeEvent);
        Task<List<Route>> GetRoutesByEmployeeIdAsync(Guid employeeId);
        Task<Route> MarkRouteAsCompletedAsync(Guid routeId);
        Task OptimizeRouteAsync(Guid routeId);

        // ✅ New - Generates routes based on company unassigned orders
        Task<IEnumerable<Route>> GenerateRoutesForCompanyAsync(Guid companyId);
    }
}
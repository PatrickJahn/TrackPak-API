using RouteService.Domain.Entities;

namespace RouteService.Domain.Interfaces;

public interface IRouteRepository
{
    Task<Route> GetRouteByIdAsync(string routeId);
    Task<List<Route>> GetRoutesByEmployeeIdAsync(string employeeId);
    Task<Route> CreateRouteAsync(Route route);
    Task UpdateRouteAsync(Route route);
    Task DeleteRouteAsync(string routeId);
}

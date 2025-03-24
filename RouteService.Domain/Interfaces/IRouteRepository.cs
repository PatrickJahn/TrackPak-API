using RouteService.Domain.Entities;

namespace RouteService.Domain.Interfaces
{
    public interface IRouteRepository
    {
        Task<Route> GetRouteByIdAsync(Guid routeId);
        Task<List<Route>> GetRoutesByEmployeeIdAsync(Guid? employeeId);
        Task<Route> CreateRouteAsync(Route route);
        Task UpdateRouteAsync(Route route);
        Task DeleteRouteAsync(Guid routeId);

        // ✅ New - Get all unassigned routes (EmployeeId == null)
        Task<List<Route>> GetUnassignedRoutesAsync();

        // ✅ New - Get all routes for a specific company
        Task<List<Route>> GetRoutesByCompanyIdAsync(Guid companyId);
    }
}
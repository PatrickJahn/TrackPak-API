using Microsoft.EntityFrameworkCore;
using RouteService.Domain.Entities;
using RouteService.Domain.Interfaces;
using RouteService.Infrastructure.DBContext;

namespace RouteService.Infrastructure.Repositories;

public class RouteRepository(RouteDbContext context) : IRouteRepository
{
    public async Task<Route> GetRouteByIdAsync(Guid routeId)
    {
        return await context.Routes.FindAsync(routeId);
    }

    public async Task<List<Route>> GetRoutesByEmployeeIdAsync(Guid? employeeId)
    {
        return await context.Routes
            .Where(r => employeeId == null ? r.EmployeeId == null : r.EmployeeId == employeeId)
            .Include(r => r.OrderRoutes)
            .ToListAsync();
    }

    public async Task<Route> CreateRouteAsync(Route route)
    {
        await context.Routes.AddAsync(route);
        await context.SaveChangesAsync();
        return route;
    }

    public async Task UpdateRouteAsync(Route route)
    {
        context.Routes.Update(route);
        await context.SaveChangesAsync();
    }

    public async Task DeleteRouteAsync(Guid routeId)
    {
        var route = await context.Routes.FindAsync(routeId);
        if (route != null)
        {
            context.Routes.Remove(route);
            await context.SaveChangesAsync();
        }
    }

    // Get unassigned routes (EmployeeId is null)
    public async Task<List<Route>> GetUnassignedRoutesAsync()
    {
        return await context.Routes
            .Where(r => r.EmployeeId == null)
            .Include(r => r.OrderRoutes)
            .ToListAsync();
    }

    //  Get all routes by CompanyId
    public async Task<List<Route>> GetRoutesByCompanyIdAsync(Guid companyId)
    {
        return await context.Routes
            .Where(r => r.CompanyId == companyId)
            .Include(r => r.OrderRoutes)
            .ToListAsync();
    }
}
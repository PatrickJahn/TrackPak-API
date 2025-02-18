using RouteService.Application.Interfaces;
using RouteService.Domain.Entities;
using RouteService.Domain.Interfaces;
using Shared.Messaging.Events.Order;

namespace RouteService.Application.Services;

public class RouteService : IRouteService
{
    private readonly IRouteRepository _routeRepository;
    private readonly IRouteOptimizer _routeOptimizer;

    public RouteService(IRouteRepository routeRepository, IRouteOptimizer routeOptimizer)
    {
        _routeRepository = routeRepository;
        _routeOptimizer = routeOptimizer;
    }

    // 1️⃣ Handles Order Created Event (Triggered by Order Service)
    public async Task HandleOrderCreatedAsync(OrderCreatedEvent orderEvent)
    {
        // Find an active route for the employee assigned to this order (if any)
        var existingRoutes = await _routeRepository.GetRoutesByEmployeeIdAsync(orderEvent.AssignedEmployeeId);

        if (existingRoutes == null)
        {
            // Create a new route for this employee
            existingRoutes = new Route
            {
                EmployeeId = orderEvent.AssignedEmployeeId,
                OrderIds = new List<string> { orderEvent.OrderId },
                Status = "Pending"
            };
            await _routeRepository.CreateRouteAsync(existingRoutes);
        }
        else
        {
            // Add order to existing route
            existingRoutes.OrderIds.Add(orderEvent.OrderId);
            await _routeRepository.UpdateRouteAsync(existingRoutes);
        }
    }

    // 2️⃣ Handles Order Cancellation Event (Triggered by Order Service)
    public async Task HandleOrderCancelledAsync(OrderCancelledEvent orderEvent)
    {
        var routes = await _routeRepository.GetAllRoutesAsync();
        var affectedRoute = routes.FirstOrDefault(r => r.OrderIds.Contains(orderEvent.OrderId));

        if (affectedRoute != null)
        {
            affectedRoute.OrderIds.Remove(orderEvent.OrderId);
            await _routeRepository.UpdateRouteAsync(affectedRoute);
        }
    }

    // 3️⃣ Handles Employee Check-In (Triggered by Employee Service)
    public async Task HandleEmployeeCheckedInAsync(EmployeeCheckedInEvent employeeEvent)
    {
        // Assign employee to any pending unassigned routes
        var pendingRoutes = await _routeRepository.GetRoutesByEmployeeIdAsync(null); // Fetch routes without employees

        if (pendingRoutes.Any())
        {
            var routeToAssign = pendingRoutes.First();
            routeToAssign.EmployeeId = employeeEvent.EmployeeId;
            await _routeRepository.UpdateRouteAsync(routeToAssign);
        }
    }

    // 4️⃣ Handles Employee Check-Out (Triggered by Employee Service)
    public async Task HandleEmployeeCheckedOutAsync(EmployeeCheckedOutEvent employeeEvent)
    {
        var activeRoutes = await _routeRepository.GetRoutesByEmployeeIdAsync(employeeEvent.EmployeeId);
        
        foreach (var route in activeRoutes)
        {
            route.Status = "Pending"; // Mark route for reassignment
            route.EmployeeId = null;
            await _routeRepository.UpdateRouteAsync(route);
        }
    }

    // 5️⃣ Fetches Routes Assigned to an Employee
    public async Task<List<Route>> GetRoutesByEmployeeIdAsync(string employeeId)
    {
        return await _routeRepository.GetRoutesByEmployeeIdAsync(employeeId);
    }

    // 6️⃣ Marks a Route as Completed
    public async Task<Route> MarkRouteAsCompletedAsync(string routeId)
    {
        var route = await _routeRepository.GetRouteByIdAsync(routeId);
        if (route == null)
            throw new Exception("Route not found.");

        route.Status = "Completed";
        route.CompletedAt = DateTime.UtcNow;
        await _routeRepository.UpdateRouteAsync(route);

        return route;
    }

    // 7️⃣ Optimizes a Route Using PythonWorker (via gRPC)
    public async Task OptimizeRouteAsync(string routeId)
    {
        var route = await _routeRepository.GetRouteByIdAsync(routeId);
        if (route == null)
            throw new Exception("Route not found.");

        await _routeOptimizer.OptimizeRoute(route);
    }
}

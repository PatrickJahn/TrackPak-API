using RouteService.Application.Interfaces;
using RouteService.Domain.Entities;
using RouteService.Domain.Interfaces;
using Shared.Messaging.Events.Order;
using Shared.Messaging.Events.Employee;
using Shared.Enums;

namespace RouteService.Application.Services;

public class RouteService(IRouteRepository routeRepository, IRouteOptimizer routeOptimizer)
    : IRouteService
{
    // 1️⃣ Handles Order Created Event (Triggered by Order Service)
    public async Task HandleOrderCreatedAsync(OrderCreatedEvent orderEvent)
    {
        var existingRoutes = await routeRepository.GetRoutesByEmployeeIdAsync(null); // Get unassigned routes

        Route routeToUse;

        if (!existingRoutes.Any())
        {
            // Create a new unassigned route
            routeToUse = new Route
            {
                EmployeeId = null,
                OrderRoutes = new List<OrderRoute> 
                { 
                    new OrderRoute { Sequence = 1, OrderId = orderEvent.OrderId }
                },
                Status = RouteStatusEnum.Pending
            };
            await routeRepository.CreateRouteAsync(routeToUse);
        }
        else
        {
            // Use an existing pending route
            routeToUse = existingRoutes.First();

            // Determine the next available sequence number
            int nextSequence = routeToUse.OrderRoutes.Any() ? routeToUse.OrderRoutes.Max(o => o.Sequence) + 1 : 1;
            routeToUse.OrderRoutes.Add(new OrderRoute { Sequence = nextSequence, OrderId = orderEvent.OrderId });

            await routeRepository.UpdateRouteAsync(routeToUse);
        }
    }

    // 2️⃣ Handles Order Cancellation Event (Triggered by Order Service)
    public async Task HandleOrderCancelledAsync(OrderCancelledEvent orderEvent)
    {
        var employeeRoutes = await routeRepository.GetRoutesByEmployeeIdAsync(null); // Fetch only unassigned routes

        var affectedRoute = employeeRoutes.FirstOrDefault(r => r.OrderRoutes.Any(o => o.OrderId == orderEvent.OrderId));

        if (affectedRoute != null)
        {
            var orderToRemove = affectedRoute.OrderRoutes.FirstOrDefault(o => o.OrderId == orderEvent.OrderId);
            if (orderToRemove != null)
            {
                affectedRoute.OrderRoutes.Remove(orderToRemove);
            }

            // If route has no more orders, delete it
            if (!affectedRoute.OrderRoutes.Any())
            {
                await routeRepository.DeleteRouteAsync(affectedRoute.Id);
            }
            else
            {
                await routeRepository.UpdateRouteAsync(affectedRoute);
            }
        }
    }

    // 3️⃣ Handles Employee Check-In (Triggered by Employee Service)
    public async Task HandleEmployeeCheckedInAsync(EmployeeCheckedInEvent employeeEvent)
    {
        // Assign employee to the first available pending route
        var pendingRoutes = await routeRepository.GetRoutesByEmployeeIdAsync(null);

        if (pendingRoutes.Any())
        {
            var routeToAssign = pendingRoutes.First();
            routeToAssign.EmployeeId = employeeEvent.EmployeeId;
            routeToAssign.Status = RouteStatusEnum.Assigned;
            await routeRepository.UpdateRouteAsync(routeToAssign);
        }
    }

    // 4️⃣ Handles Employee Check-Out (Triggered by Employee Service)
    public async Task HandleEmployeeCheckedOutAsync(EmployeeCheckedOutEvent employeeEvent)
    {
        var activeRoutes = await routeRepository.GetRoutesByEmployeeIdAsync(employeeEvent.EmployeeId);

        foreach (var route in activeRoutes)
        {
            route.Status = RouteStatusEnum.Pending;
            route.EmployeeId = null;
            await routeRepository.UpdateRouteAsync(route);
        }
    }

    // 5️⃣ Fetches Routes Assigned to an Employee
    public async Task<List<Route>> GetRoutesByEmployeeIdAsync(Guid employeeId)
    {
        return await routeRepository.GetRoutesByEmployeeIdAsync(employeeId);
    }

    // 6️⃣ Marks a Route as Completed
    public async Task<Route> MarkRouteAsCompletedAsync(Guid routeId)
    {
        var route = await routeRepository.GetRouteByIdAsync(routeId);
        if (route == null)
            throw new Exception("Route not found.");

        route.Status = RouteStatusEnum.Completed;
        route.CompletedAt = DateTime.UtcNow;
        await routeRepository.UpdateRouteAsync(route);

        return route;
    }

    // 7️⃣ Optimizes a Route Using PythonWorker (via gRPC)
    public async Task OptimizeRouteAsync(Guid routeId)
    {
        var route = await routeRepository.GetRouteByIdAsync(routeId);
        if (route == null)
            throw new Exception("Route not found.");

        await routeOptimizer.OptimizeRoute(route);
    }
}

using RouteService.Domain.Entities;

namespace RouteService.Domain.Interfaces;

public interface IRouteOptimizer
{
    Task OptimizeRoute(Route route);
}

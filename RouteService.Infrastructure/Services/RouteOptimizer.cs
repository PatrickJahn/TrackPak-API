using RouteService.Domain.Entities;
using RouteService.Domain.Interfaces;

namespace RouteService.Infrastructure.Services;

public class RouteOptimizer(GrpcRouteOptimizerClient grpcClient) : IRouteOptimizer
{
    private readonly GrpcRouteOptimizerClient _grpcClient = grpcClient;

    public async Task OptimizeRoute(Route route)
    {
        await _grpcClient.OptimizeRouteAsync(route);
    }
}

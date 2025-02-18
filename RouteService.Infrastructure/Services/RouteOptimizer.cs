using RouteService.Domain.Entities;
using RouteService.Domain.Interfaces;
using RouteService.Infrastructure.Grpc;

namespace RouteService.Infrastructure.Services;

public class RouteOptimizer(GrpcRouteOptimizerClient grpcClient) : IRouteOptimizer
{

    public async Task OptimizeRoute(Route route)
    {
        await grpcClient.OptimizeRouteAsync(route);
    }
}
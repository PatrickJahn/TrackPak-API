using Grpc.Net.Client;
using RouteService.Grpc;
using RouteService.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace RouteService.Infrastructure.Grpc;

public class GrpcRouteOptimizerClient
{
    private readonly RouteOptimizerService.RouteOptimizerServiceClient _client;

    public GrpcRouteOptimizerClient(GrpcChannel channel)
    {
        _client = new RouteOptimizerService.RouteOptimizerServiceClient(channel);
    }

    public async Task<bool> OptimizeRouteAsync(Route route)
    {
        var request = new OptimizeRouteRequest
        {
            RouteId = route.Id.ToString(),
        };

        // ✅ Update to use List<OrderRoute> instead of Dictionary<int, Guid>
        request.OrderIds.AddRange(route.OrderRoutes.Select(o => o.OrderId.ToString()));

        try
        {
            var response = await _client.OptimizeRouteAsync(request);
            return response.Success;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error calling gRPC RouteOptimizer: {ex.Message}");
            return false;
        }
    }
}
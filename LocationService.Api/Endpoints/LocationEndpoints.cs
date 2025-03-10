using LocationService.Application.Interfaces;
using Shared.Models;

namespace LocationService.Api.Endpoints;

public static class LocationEndpoints
{
    public static void MapLocationEndpoints(this IEndpointRouteBuilder app)
    {
        
        // TODO: Add GetOrCreateEndpoint
        
        app.MapGet("location/{id}", async (Guid id, ILocationService service) =>
        {
            var location = await service.GetLocationByIdAsync(id);
            return Results.Ok(location);
        });
        
        app.MapPost("location", async (CreateLocationRequestModel locationModel, ILocationService service) =>
        {
            var location = await service.CreateLocationAsync(locationModel);
            return Results.Ok(location.Id);
        });
        
        app.MapDelete("location/{id}", async (Guid id, ILocationService service) =>
        {
            await service.DeleteLocationAsync(id); 
            return Results.Ok();
        });

        
    }
}
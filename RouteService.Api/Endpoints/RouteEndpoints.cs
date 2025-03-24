
using RouteService.Application.Interfaces;
using Shared.Extensions;

namespace RouteService.Api.Endpoints
{
    public static class RouteEndpoints
    {
        public static void MapRouteEndpoints(this IEndpointRouteBuilder app)
        {
            //  Get Route by ID
            app.MapGet("route/{id}", async (Guid id, IRouteService service) =>
            {
                var route = await service.GetRoutesByEmployeeIdAsync(id);
                return route is not null ? Results.Ok(route) : Results.NotFound();
            });

            //  Get All Routes for a Specific Employee
            app.MapGet("/route/employee/routes", async (HttpContext httpContext, IRouteService service) =>
            {
                var employeeId = httpContext.GetEmployeeId() ?? Guid.Empty;

                var routes = await service.GetRoutesByEmployeeIdAsync(employeeId);
                return Results.Ok(routes);
            });
            //  Get All Routes for a Specific Company
            app.MapGet("/route/company/routes", async (HttpContext httpContext, IRouteService service) =>
            {
                var companyId = httpContext.GetCompanyId() ?? Guid.Empty;
                var routes = await service.GetRoutesByEmployeeIdAsync(companyId);
                return Results.Ok(routes);
            });
            //  Mark Route as Completed
            app.MapPost("route/{id:guid}/complete", async (Guid id, IRouteService service) =>
            {
                var route = await service.MarkRouteAsCompletedAsync(id);
                return route is not null ? Results.Ok(route) : Results.NotFound();
            });

            //  Optimize Route
            app.MapPost("route/{id:guid}/optimize", async (Guid id, IRouteService service) =>
            {
                try
                {
                    await service.OptimizeRouteAsync(id);
                    return Results.Ok("Route optimization started.");
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Optimization failed: {ex.Message}");
                }
            });

            //  Delete Route by ID
            app.MapDelete("route/{id:guid}", async (Guid id, IRouteService service) =>
            {
                var route = await service.MarkRouteAsCompletedAsync(id);
                if (route is null)
                {
                    return Results.NotFound();
                }

                await service.MarkRouteAsCompletedAsync(id);
                return Results.Ok($"Route {id} deleted successfully.");
            });
            
            // ✅ Generate Routes for a Company
            app.MapPost("/route/company/generate", async (HttpContext httpContext, IRouteService service) =>
            {
                var companyId = httpContext.GetCompanyId();
                if (companyId is null)
                {
                    return Results.BadRequest("Missing company ID.");
                }

                try
                {
                    var generatedRoutes = await service.GenerateRoutesForCompanyAsync(companyId.Value);
                    return Results.Ok(generatedRoutes);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Failed to generate routes: {ex.Message}");
                }
            });

        }
        
        
    }
}

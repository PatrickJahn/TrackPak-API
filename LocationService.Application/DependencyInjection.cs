using LocationService.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace LocationService.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddHttpClient<IGeocodingService, Services.GeocodingService>();
        services.AddScoped<ILocationService, Services.LocationService>();
    }
}
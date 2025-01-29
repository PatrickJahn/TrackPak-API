using LocationService.Domain.Entities;

namespace LocationService.Application.Interfaces;

public interface IGeocodingService
{
    public Task<GeoLocation?> GetGeoLocationAsync(string address);

}
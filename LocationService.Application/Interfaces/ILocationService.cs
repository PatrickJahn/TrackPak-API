using LocationService.Domain.Entities;
using Shared.Models;

namespace LocationService.Application.Interfaces;

public interface ILocationService
{
    Task<Location> GetLocationByIdAsync(Guid id);
    Task<IEnumerable<Location>> GetAllLocationsAsync();
    Task<Location> CreateLocationAsync(CreateLocationRequestModel locationRequest);
    Task DeleteLocationAsync(Guid id);
}

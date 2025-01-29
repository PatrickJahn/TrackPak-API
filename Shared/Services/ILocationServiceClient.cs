using Shared.Models;

namespace Shared.Services;

public interface ILocationServiceClient
{
    Task<LocationResponseModel?> GetLocationByIdAsync(Guid locationId);
    Task<Guid?> CreateLocationAsync(CreateLocationRequestModel request);
}
using LocationService.Application.Interfaces;
using LocationService.Application.Interfaces.Repositories;
using LocationService.Domain.Entities;
using Shared.Models;

namespace LocationService.Application.Services;

public class LocationService: ILocationService
{
    
    private readonly ILocationRepository _locationRepo;
    private readonly IGeocodingService _geocodingService;


    public LocationService(ILocationRepository locationRepo, IGeocodingService geocodingService)
    {
        _locationRepo = locationRepo ?? throw new ArgumentNullException(nameof(locationRepo));
        _geocodingService = geocodingService ?? throw new ArgumentNullException(nameof(geocodingService));
    }

    public async Task<Location> GetLocationByIdAsync(Guid id)
    {
        return await _locationRepo.GetByIdAsync(id);
         
    }

    public async Task<IEnumerable<Location>> GetAllLocationsAsync()
    {
        return await _locationRepo.GetAllAsync();
    }


    public async Task<Location> CreateLocationAsync(CreateLocationModel model)
    {
        
        var fullAddress = $"{model.AddressLine}, {model.City}, {model.Country}, {model.PostalCode}";
        var geoLocation = await _geocodingService.GetGeoLocationAsync(fullAddress);

        var location = new Location
        {
            Country = model.Country,
            City = model.City,
            AddressLine = model.AddressLine,
            PostalCode = model.PostalCode,
            GeoLocation = geoLocation
        };

        await _locationRepo.AddAsync(location);
        return location;
    }

    
    public async Task DeleteLocationAsync(Guid id)
    {
        await _locationRepo.DeleteByIdAsync(id);
    }
    
   

}
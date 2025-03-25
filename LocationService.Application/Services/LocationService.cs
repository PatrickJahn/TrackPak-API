using System.Diagnostics;
using LocationService.Application.Interfaces;
using LocationService.Application.Interfaces.Repositories;
using LocationService.Domain.Entities;
using Monitoring;
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

    public async Task<Location?> GetLocationByIdAsync(Guid id)
    {
        using var activity = LoggingService.activitySource.StartActivity("LocationService.GetLocationByIdAsync", ActivityKind.Internal);
        activity?.SetTag("location.id", id);

        LoggingService.Log.AddContext().Information("Fetching location by ID: {LocationId}", id);
        return await _locationRepo.GetOrDefaultByIdAsync(id);
         
    }

    public async Task<IEnumerable<Location>> GetAllLocationsAsync()
    {
        using var activity = LoggingService.activitySource.StartActivity("LocationService.GetAllLocationsAsync", ActivityKind.Internal);

        LoggingService.Log.AddContext().Information("Fetching all locations");
        return await _locationRepo.GetAllAsync();
    }


    public async Task<Location> CreateLocationAsync(CreateLocationRequestModel requestModel)
    {
        using var activity = LoggingService.activitySource.StartActivity("LocationService.CreateLocationAsync", ActivityKind.Internal);

        activity?.SetTag("location.city", requestModel.City);
        activity?.SetTag("location.country", requestModel.Country);
        activity?.SetTag("location.address", requestModel.AddressLine);

        LoggingService.Log.AddContext().Information("Creating location: {@Request}", requestModel);
        
        var fullAddress = $"{requestModel.AddressLine}, {requestModel.City}, {requestModel.Country}, {requestModel.PostalCode}";
        var geoLocation = await _geocodingService.GetGeoLocationAsync(fullAddress);

        var location = new Location
        {
            Country = requestModel.Country,
            City = requestModel.City,
            AddressLine = requestModel.AddressLine,
            PostalCode = requestModel.PostalCode,
            GeoLocation = geoLocation,
        };

        await _locationRepo.AddAsync(location);
        return location;
    }

    
    public async Task DeleteLocationAsync(Guid id)
    {
        
        using var activity = LoggingService.activitySource.StartActivity("LocationService.DeleteLocationAsync", ActivityKind.Internal);
        activity?.SetTag("location.id", id);

        LoggingService.Log.AddContext().Information("Deleting location: {LocationId}", id);

        await _locationRepo.DeleteByIdAsync(id);
    }
    
   

}
using LocationService.Application.Interfaces;
using LocationService.Application.Interfaces.Repositories;
using LocationService.Domain.Entities;
using Moq;
using Shared.Models;
using GeoLocation = LocationService.Domain.Entities.GeoLocation;

namespace LocationService.Tests;

[Trait("Category", "UnitTests")]
public class LocationServiceTests
{
    private readonly Mock<ILocationRepository> _locationRepoMock;
    private readonly Mock<IGeocodingService> _geocodingServiceMock;
    private readonly Application.Services.LocationService _locationService;

    public LocationServiceTests()
    {
        _locationRepoMock = new Mock<ILocationRepository>();
        _geocodingServiceMock = new Mock<IGeocodingService>();
        _locationService = new Application.Services.LocationService(_locationRepoMock.Object, _geocodingServiceMock.Object);
    }

    [Fact]
    public async Task GetLocationByIdAsync_ShouldReturnLocation_WhenLocationExists()
    {
        // Arrange
        var locationId = Guid.NewGuid();
        var expectedLocation = new Location { Id = locationId };
        _locationRepoMock.Setup(repo => repo.GetOrDefaultByIdAsync(locationId))
                         .ReturnsAsync(expectedLocation);

        // Act
        var result = await _locationService.GetLocationByIdAsync(locationId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(locationId, result.Id);
        _locationRepoMock.Verify(repo => repo.GetOrDefaultByIdAsync(locationId), Times.Once);
    }

    [Fact]
    public async Task GetAllLocationsAsync_ShouldReturnAllLocations()
    {
        // Arrange
        var locations = new List<Location>
        {
            new Location { Id = Guid.NewGuid() },
            new Location { Id = Guid.NewGuid() }
        };
        _locationRepoMock.Setup(repo => repo.GetAllAsync(default))
                         .ReturnsAsync(locations);

        // Act
        var result = await _locationService.GetAllLocationsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _locationRepoMock.Verify(repo => repo.GetAllAsync(default), Times.Once);
    }

    [Fact]
    public async Task CreateLocationAsync_ShouldCreateLocationAndCallRepo()
    {
        // Arrange
        var requestModel = new CreateLocationRequestModel
        {
            Country = "Denmark",
            City = "Copenhagen",
            AddressLine = "Some Street 123",
            PostalCode = "1000"
        };
        var geoLocation = new GeoLocation { Latitude = 55.6761, Longitude = 12.5683 };

        _geocodingServiceMock.Setup(service => service.GetGeoLocationAsync(It.IsAny<string>()))
                             .ReturnsAsync(geoLocation);

        // Act
        var result = await _locationService.CreateLocationAsync(requestModel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(requestModel.City, result.City);
        Assert.Equal(requestModel.Country, result.Country);
        Assert.Equal(requestModel.AddressLine, result.AddressLine);
        Assert.Equal(requestModel.PostalCode, result.PostalCode);
        Assert.Equal(geoLocation, result.GeoLocation);

        _geocodingServiceMock.Verify(service => service.GetGeoLocationAsync(It.IsAny<string>()), Times.Once);
        _locationRepoMock.Verify(repo => repo.AddAsync(It.IsAny<Location>()), Times.Once);
    }

    [Fact]
    public async Task CreateLocationAsync_ShouldCallGetGeoLocationWithCorrectAddress()
    {
        // Arrange
        var requestModel = new CreateLocationRequestModel
        {
            Country = "Denmark",
            City = "Copenhagen",
            AddressLine = "Some Street 123",
            PostalCode = "1000"
        };
        var expectedAddress = "Some Street 123, Copenhagen, Denmark, 1000";

        _geocodingServiceMock.Setup(service => service.GetGeoLocationAsync(expectedAddress))
                             .ReturnsAsync(new GeoLocation());

        // Act
        await _locationService.CreateLocationAsync(requestModel);

        // Assert
        _geocodingServiceMock.Verify(service => service.GetGeoLocationAsync(expectedAddress), Times.Once);
    }

    [Fact]
    public async Task DeleteLocationAsync_ShouldCallRepository()
    {
        // Arrange
        var locationId = Guid.NewGuid();

        // Act
        await _locationService.DeleteLocationAsync(locationId);

        // Assert
        _locationRepoMock.Verify(repo => repo.DeleteByIdAsync(locationId), Times.Once);
    }
}
using LocationService.Application.Interfaces;
using LocationService.Application.Messaging.Handlers;
using LocationService.Domain.Entities;
using Moq;
using Shared.Messaging.Events.Company;
using Shared.Messaging.Events.Location;
using Shared.Models;

namespace LocationService.Tests;

public class CompanyCreatedHandlerTests
{
    private readonly Mock<ILocationService> _locationServiceMock;
    private readonly Mock<ILocationEventPublisher> _locationEventPublisherMock;
    private readonly CompanyCreatedHandler _handler;

    public CompanyCreatedHandlerTests()
    {
        _locationServiceMock = new Mock<ILocationService>();
        _locationEventPublisherMock = new Mock<ILocationEventPublisher>();
        _handler = new CompanyCreatedHandler(_locationServiceMock.Object, _locationEventPublisherMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateLocation_AndPublishEvent()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var locationId = Guid.NewGuid();
        var locationRequest = new CreateLocationRequestModel { City = "Copenhagen" };
        var createdLocation = new Location { Id = locationId, City = "Copenhagen" };

        var message = new CompanyCreatedEvent { CompanyId = companyId, Location = locationRequest };

        _locationServiceMock.Setup(service => service.CreateLocationAsync(locationRequest))
            .ReturnsAsync(createdLocation);

        // Act
        await _handler.HandleAsync(message, CancellationToken.None);

        // Assert
        _locationServiceMock.Verify(service => service.CreateLocationAsync(locationRequest), Times.Once);

        _locationEventPublisherMock.Verify(publisher => 
                publisher.PublishCompanyLocationCreatedAsync(
                    It.Is<CompanyLocationCreatedEvent>(e => e.CompanyId == companyId && e.LocationId == locationId)
                ),
            Times.Once);
    }
}
using LocationService.Application.Interfaces;
using LocationService.Application.Messaging.Handlers;
using LocationService.Domain.Entities;
using Moq;
using Shared.Messaging.Events.Company;
using Shared.Messaging.Events.Location;
using Shared.Messaging.Events.User;
using Shared.Models;

namespace LocationService.Tests;

public class UserCreatedHandlerTests
{
    private readonly Mock<ILocationService> _locationServiceMock;
    private readonly Mock<ILocationEventPublisher> _locationEventPublisherMock;
    private readonly UserCreatedHandler _handler;

    public UserCreatedHandlerTests()
    {
        _locationServiceMock = new Mock<ILocationService>();
        _locationEventPublisherMock = new Mock<ILocationEventPublisher>();
        _handler = new UserCreatedHandler(_locationServiceMock.Object, _locationEventPublisherMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateLocation_AndPublishEvent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var locationId = Guid.NewGuid();
        var locationRequest = new CreateLocationRequestModel { City = "Copenhagen" };
        var createdLocation = new Location { Id = locationId, City = "Copenhagen" };

        var message = new UserCreatedEvent() { UserId = userId, Location = locationRequest };

        _locationServiceMock.Setup(service => service.CreateLocationAsync(locationRequest))
            .ReturnsAsync(createdLocation);

        // Act
        await _handler.HandleAsync(message, CancellationToken.None);

        // Assert
        _locationServiceMock.Verify(service => service.CreateLocationAsync(locationRequest), Times.Once);

        _locationEventPublisherMock.Verify(publisher => 
                publisher.PublishUserLocationCreatedAsync(
                    It.Is<UserLocationCreatedEvent>(e => e.UserId == userId && e.LocationId == locationId)
                ),
            Times.Once);
    }
}
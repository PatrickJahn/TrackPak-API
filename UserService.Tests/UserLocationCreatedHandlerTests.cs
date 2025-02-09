using FluentAssertions;
using Moq;
using Shared.Messaging.Events.Location;
using UserService.Application.Messaging.Handlers;
using UserService.Application.Repositories;
using UserService.Domain.entities;

namespace UserService.Tests;

public class UserLocationCreatedHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UserLocationCreatedHandler _handler;

    public UserLocationCreatedHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new UserLocationCreatedHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateUserLocation_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var locationId = Guid.NewGuid();
        var user = new User { Id = userId };

        var message = new UserLocationCreatedEvent
        {
            UserId = userId,
            LocationId = locationId
        };

        _mockUserRepository.Setup(repo => repo.GetOrDefaultByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        await _handler.HandleAsync(message, CancellationToken.None);

        // Assert
        user.LocationId.Should().Be(locationId);
        _mockUserRepository.Verify(repo => repo.Update(user), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldNotUpdateUserLocation_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var locationId = Guid.NewGuid();
        var message = new UserLocationCreatedEvent
        {
            UserId = userId,
            LocationId = locationId
        };

        _mockUserRepository.Setup(repo => repo.GetOrDefaultByIdAsync(userId))
            .ReturnsAsync((User)null);

        // Act
        await _handler.HandleAsync(message, CancellationToken.None);

        // Assert
        _mockUserRepository.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
    }
}

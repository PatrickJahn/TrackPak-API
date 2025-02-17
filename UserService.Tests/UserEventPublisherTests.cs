using Moq;
using Shared.Messaging;
using Shared.Messaging.Events.User;
using Shared.Messaging.Topics;
using Shared.Models;
using UserService.Application.Messaging;
using UserService.Application.Models;
using UserService.Domain.entities;
using UserService.Infrastructure.Messaging;

namespace UserService.Tests;

[Trait("Category", "UnitTests")]
public class UserEventPublisherTests
{
    private readonly Mock<IMessageBus> _messageBusMock;
    private readonly UserEventPublisher _eventPublisher;

    public UserEventPublisherTests()
    {
        _messageBusMock = new Mock<IMessageBus>();
        _eventPublisher = new UserEventPublisher(_messageBusMock.Object);
    }

    [Fact]
    public async Task PublishUserCreatedAsync_Should_Call_MessageBus_With_Correct_Event()
    {
        // Arrange
        var user = new User
        {
            PhoneNumber = "123456789",
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };

        var location = new CreateLocationRequestModel();

        _messageBusMock
            .Setup(bus => bus.PublishAsync(It.IsAny<MessageTopic>(), It.IsAny<UserCreatedEvent>()))
            .Returns(Task.CompletedTask);

        // Act
        await _eventPublisher.PublishUserCreatedAsync(user, location);

        // Assert
        _messageBusMock.Verify(bus => bus.PublishAsync(
            MessageTopic.UserCreated,
            It.Is<UserCreatedEvent>(e =>
                e.Email == user.Email &&
                e.PhoneNumber == user.PhoneNumber &&
                e.FirstName == user.FirstName &&
                e.LastName == user.LastName &&
                e.Location == location)
        ), Times.Once);
        
    }
}
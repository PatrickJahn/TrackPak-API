using Moq;
using Shared.Messaging;
using Shared.Messaging.Events.User;
using Shared.Messaging.Topics;
using UserService.Application.Messaging;
using UserService.Application.Models;
using UserService.Infrastructure.Messaging;

namespace UserService.Tests;

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
        var userModel = new CreateUserModel
        {
            PhoneNumber = "123456789",
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Location = {}
        };

        _messageBusMock
            .Setup(bus => bus.PublishAsync(It.IsAny<MessageTopic>(), It.IsAny<UserCreatedEvent>()))
            .Returns(Task.CompletedTask);

        // Act
        await _eventPublisher.PublishUserCreatedAsync(userModel);

        // Assert
        _messageBusMock.Verify(bus => bus.PublishAsync(
            MessageTopic.UserCreated,
            It.Is<UserCreatedEvent>(e =>
                e.Email == userModel.Email &&
                e.PhoneNumber == userModel.PhoneNumber &&
                e.FirstName == userModel.FirstName &&
                e.LastName == userModel.LastName &&
                e.Location == userModel.Location)
        ), Times.Once);
        
    }
}
using LocationService.Application.Messaging;
using Moq;
using Shared.Messaging;
using Shared.Messaging.Events.Location;
using Shared.Messaging.Topics;

namespace LocationService.Tests;

public class LocationEventPublisherTests{

private readonly Mock<IMessageBus> _messageBusMock;
private readonly LocationEventPublisher _publisher;

public LocationEventPublisherTests()
{
    _messageBusMock = new Mock<IMessageBus>();
    _publisher = new LocationEventPublisher(_messageBusMock.Object);
}

[Fact]
public async Task PublishUserLocationCreatedAsync_Should_Call_MessageBus_With_Correct_Topic()
{
    // Arrange
    var message = new UserLocationCreatedEvent();

    // Act
    await _publisher.PublishUserLocationCreatedAsync(message);

    // Assert
    _messageBusMock.Verify(mb => mb.PublishAsync(MessageTopic.UserLocationCreated, message), Times.Once);
}

[Fact]
public async Task PublishOrderLocationCreatedAsync_Should_Call_MessageBus_With_Correct_Topic()
{
    // Arrange
    var message = new OrderLocationCreatedEvent();

    // Act
    await _publisher.PublishOrderLocationCreatedAsync(message);

    // Assert
    _messageBusMock.Verify(mb => mb.PublishAsync(MessageTopic.OrderLocationCreated, message), Times.Once);
}

[Fact]
public async Task PublishCompanyLocationCreatedAsync_Should_Call_MessageBus_With_Correct_Topic()
{
    // Arrange
    var message = new CompanyLocationCreatedEvent();

    // Act
    await _publisher.PublishCompanyLocationCreatedAsync(message);

    // Assert
    _messageBusMock.Verify(mb => mb.PublishAsync(MessageTopic.CompanyLocationCreated, message), Times.Once);
}

[Fact]
public async Task PublishEmployeeLocationCreatedAsync_Should_Call_MessageBus_With_Correct_Topic()
{
    // Arrange
    var message = new EmployeeLocationCreatedEvent();

    // Act
    await _publisher.PublishEmployeeLocationCreatedAsync(message);

    // Assert
    _messageBusMock.Verify(mb => mb.PublishAsync(MessageTopic.EmployeeLocationCreated, message), Times.Once);
}
}
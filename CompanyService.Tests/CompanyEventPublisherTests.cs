using CompanyService.Domain.Entities;
using CompanyService.Infrastructure.Messaging;
using Moq;
using Shared.Messaging;
using Shared.Messaging.Events.Company;
using Shared.Messaging.Topics;
using Shared.Models;

namespace CompanyService.Tests;

public class CompanyEventPublisherTests
{
    private readonly Mock<IMessageBus> _messageBusMock;
    private readonly CompanyEventPublisher _publisher;

    public CompanyEventPublisherTests()
    {
        _messageBusMock = new Mock<IMessageBus>();
        _publisher = new CompanyEventPublisher(_messageBusMock.Object);
    }

    [Fact]
    public async Task PublishCompanyCreatedAsync_ShouldPublishEvent_WithCorrectData()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Test Company"
        };

        var locationRequest = new CreateLocationRequestModel
        {
            City = "Copenhagen",
            Country = "Denmark"
        };

        // Act
        await _publisher.PublishCompanyCreatedAsync(company, locationRequest);

        // Assert
        _messageBusMock.Verify(
            bus => bus.PublishAsync(
                MessageTopic.CompanyCreated,
                It.Is<CompanyCreatedEvent>(e =>
                    e.CompanyId == company.Id &&
                    e.Location.City == locationRequest.City &&
                    e.Location.Country == locationRequest.Country
                )
            ),
            Times.Once
        );
    }
}

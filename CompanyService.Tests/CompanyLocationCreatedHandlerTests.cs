using CompanyService.Application.Messaging.Handlers;
using CompanyService.Domain.Entities;
using CompanyService.Domain.Interfaces;
using Moq;
using Shared.Messaging.Events.Location;

namespace CompanyService.Tests;

public class CompanyLocationCreatedHandlerTests
{
    private readonly Mock<ICompanyRepository> _companyRepositoryMock;
    private readonly CompanyLocationCreatedHandler _handler;

    public CompanyLocationCreatedHandlerTests()
    {
        _companyRepositoryMock = new Mock<ICompanyRepository>();
        _handler = new CompanyLocationCreatedHandler(_companyRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateCompany_WhenCompanyExists()
    {
        // Arrange
        var message = new CompanyLocationCreatedEvent
        {
            CompanyId = Guid.NewGuid(),
            LocationId = Guid.NewGuid()
        };

        var existingCompany = new Company
        {
            Id = message.CompanyId,
            LocationId = null
        };

        _companyRepositoryMock
            .Setup(repo => repo.GetOrDefaultByIdAsync(message.CompanyId))
            .ReturnsAsync(existingCompany);

        // Act
        await _handler.HandleAsync(message, CancellationToken.None);

        // Assert
        _companyRepositoryMock.Verify(
            repo => repo.Update(It.Is<Company>(c => c.Id == message.CompanyId && c.LocationId == message.LocationId)), 
            Times.Once
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldNotUpdate_WhenCompanyDoesNotExist()
    {
        // Arrange
        var message = new CompanyLocationCreatedEvent
        {
            CompanyId = Guid.NewGuid(),
            LocationId = Guid.NewGuid()
        };

        _companyRepositoryMock
            .Setup(repo => repo.GetOrDefaultByIdAsync(message.CompanyId))
            .ReturnsAsync((Company?)null);

        // Act
        await _handler.HandleAsync(message, CancellationToken.None);

        // Assert
        _companyRepositoryMock.Verify(repo => repo.Update(It.IsAny<Company>()), Times.Never);
    }
}

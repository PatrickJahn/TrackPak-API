using Moq;
using System.Threading;
using System.Threading.Tasks;
using CompanyService.Application.Interfaces;
using CompanyService.Domain.Entities;
using Xunit;
using System.Collections.Generic;
using CompanyService.Application.Models;
using CompanyService.Domain.Interfaces;
using Shared.Models;

namespace CompanyService.Tests;

public class CompanyServiceTests
    {
        
       private readonly Mock<ICompanyRepository> _mockCompanyRepository;
       private readonly Mock<ICompanyEventPublisher> _companyEventPublisherMock;

       private readonly Application.Services.CompanyService _companyService;

    public CompanyServiceTests()
    {
        _mockCompanyRepository = new Mock<ICompanyRepository>();
        _companyEventPublisherMock = new Mock<ICompanyEventPublisher>();
        _companyService = new Application.Services.CompanyService(_mockCompanyRepository.Object, _companyEventPublisherMock.Object);
    }

    [Fact]
    public async Task CreateCompanyAsync_ShouldReturnCreatedCompany()
    {
        // Arrange
        var companyModel = new CreateCompanyModel
        {
            Name = "Test Company",
            Cvr = "12345678",
            BrandId = Guid.NewGuid().ToString()
        };

        Company capturedCompany = null!;
        _mockCompanyRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Company>()))
            .Callback<Company>(c => capturedCompany = c)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _companyService.CreateCompanyAsync(companyModel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(companyModel.Name, capturedCompany.Name);
        Assert.Equal(companyModel.Cvr, capturedCompany.Cvr);
        Assert.Equal(companyModel.BrandId, capturedCompany.BrandId);
        Assert.Null(capturedCompany.LocationId);
        Assert.True(capturedCompany.CreatedAt <= DateTime.UtcNow);
        _mockCompanyRepository.Verify(repo => repo.AddAsync(It.IsAny<Company>()), Times.Once);
    }

    [Fact]
    public async Task GetCompanyByIdAsync_ShouldReturnCompany_WhenCompanyExists()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var company = new Company { Id = companyId, Name = "Test Company" };

        _mockCompanyRepository
            .Setup(repo => repo.GetOrDefaultByIdAsync(companyId))
            .ReturnsAsync(company);

        // Act
        var result = await _companyService.GetCompanyByIdAsync(companyId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(companyId, result.Id);
        _mockCompanyRepository.Verify(repo => repo.GetOrDefaultByIdAsync(companyId), Times.Once);
    }

    [Fact]
    public async Task GetCompanyByIdAsync_ShouldReturnNull_WhenCompanyDoesNotExist()
    {
        // Arrange
        var companyId = Guid.NewGuid();

        _mockCompanyRepository
            .Setup(repo => repo.GetOrDefaultByIdAsync(companyId))
            .ReturnsAsync((Company?)null);

        // Act
        var result = await _companyService.GetCompanyByIdAsync(companyId);

        // Assert
        Assert.Null(result);
        _mockCompanyRepository.Verify(repo => repo.GetOrDefaultByIdAsync(companyId), Times.Once);
    }

    [Fact]
    public async Task GetCompaniesAsync_ShouldReturnListOfCompanies()
    {
        // Arrange
        var companies = new List<Company>
        {
            new Company { Id = Guid.NewGuid(), Name = "Company A" },
            new Company { Id = Guid.NewGuid(), Name = "Company B" }
        };

        _mockCompanyRepository
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(companies);

        // Act
        var result = await _companyService.GetCompaniesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockCompanyRepository.Verify(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCompanyAsync_ShouldReturnTrue_WhenCompanyExists()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var existingCompany = new Company { Id = companyId, Name = "Old Name", Cvr = "1111", BrandId = Guid.NewGuid().ToString() };
        var updatedCompany = new UpdateCompanyModel { Name = "New Name", Cvr = "2222", BrandId = Guid.NewGuid().ToString() };

        _mockCompanyRepository
            .Setup(repo => repo.GetByIdAsync(companyId))
            .ReturnsAsync(existingCompany);

        _mockCompanyRepository
            .Setup(repo => repo.Update(It.IsAny<Company>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _companyService.UpdateCompanyAsync(companyId, updatedCompany);

        // Assert
        Assert.True(result);
        Assert.Equal(updatedCompany.Name, existingCompany.Name);
        Assert.Equal(updatedCompany.Cvr, existingCompany.Cvr);
        Assert.Equal(updatedCompany.BrandId, existingCompany.BrandId);
        _mockCompanyRepository.Verify(repo => repo.GetByIdAsync(companyId), Times.Once);
        _mockCompanyRepository.Verify(repo => repo.Update(It.IsAny<Company>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCompanyAsync_ShouldThrowException_WhenCompanyDoesNotExist()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var updatedCompany = new UpdateCompanyModel { Name = "New Name", Cvr = "2222", BrandId = Guid.NewGuid().ToString() };

        _mockCompanyRepository
            .Setup(repo => repo.GetOrDefaultByIdAsync(companyId))
            .ReturnsAsync((Company?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() =>
            _companyService.UpdateCompanyAsync(companyId, updatedCompany));
    }

    [Fact]
    public async Task DeleteCompanyAsync_ShouldReturnTrue_WhenCompanyExists()
    {
        // Arrange
        var companyId = Guid.NewGuid();

        _mockCompanyRepository
            .Setup(repo => repo.DeleteByIdAsync(companyId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _companyService.DeleteCompanyAsync(companyId);

        // Assert
        Assert.True(result);
        _mockCompanyRepository.Verify(repo => repo.DeleteByIdAsync(companyId), Times.Once);
    }
    
    [Fact]
    public async Task CreateCompanyAsync_ShouldCallEventPublisher_WhenCompanyIsCreated()
    {
        // Arrange
        var model = new CreateCompanyModel
        {
            Name = "Test Company",
            Cvr = "12345678",
            BrandId = "Brand123",
            Location = new CreateLocationRequestModel
            {
                City = "Copenhagen",
                Country = "Denmark"
            }
        };

        _mockCompanyRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Company>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _companyService.CreateCompanyAsync(model, CancellationToken.None);

        // Assert
        _companyEventPublisherMock.Verify(
            publisher => publisher.PublishCompanyCreatedAsync(
                It.Is<Company>(c => c.Cvr == model.Cvr && c.Name == model.Name), 
                It.Is<CreateLocationRequestModel>(l => l.City == model.Location.City && l.Country == model.Location.Country)
            ), 
            Times.Once
        );
    }
}

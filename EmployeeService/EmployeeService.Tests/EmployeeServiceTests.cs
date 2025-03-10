using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Shared.Models;
using Shared.Services;
using EmployeeService.Application.Models;
using EmployeeService.Application.Repositories;
using EmployeeService.Domain.Entities;

namespace EmployeeService.Tests;

public class EmployeeServiceTests
{
    private readonly Mock<IEmployeeRepository> _employeeRepoMock;

    private readonly EmployeeService.Application.Services.EmployeeService _employeeService;

    public EmployeeServiceTests()
    {
        _employeeRepoMock = new Mock<IEmployeeRepository>();

        _employeeService = new EmployeeService.Application.Services.EmployeeService(_employeeRepoMock.Object);
    }

    [Fact]
    public async Task GetEmployeeByIdAsync_ShouldReturnEmployee_WhenEmployeeExists()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var expectedEmployee = new Employee { Id = employeeId, FirstName = "John", LastName = "Doe" };

        _employeeRepoMock.Setup(repo => repo.GetByIdAsync(employeeId))
                         .ReturnsAsync(expectedEmployee);

        // Act
        var result = await _employeeService.GetEmployeeByIdAsync(employeeId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(employeeId);
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");

        _employeeRepoMock.Verify(repo => repo.GetByIdAsync(employeeId), Times.Once);
    }

    [Fact]
    public async Task UpdateEmployeeAsync_ShouldUpdateAndReturnEmployee()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var existingEmployee = new Employee { Id = employeeId, FirstName = "John", LastName = "Doe", Email = "old@example.com" };
        var updateModel = new UpdateEmployeeModel { FirstName = "Jane", LastName = "Smith", Email = "new@example.com", PhoneNumber = "123456789" };

        _employeeRepoMock.Setup(repo => repo.GetByIdAsync(employeeId))
                         .ReturnsAsync(existingEmployee);
        _employeeRepoMock.Setup(repo => repo.Update(It.IsAny<Employee>()))
                         .Returns(Task.CompletedTask);

        // Act
        var result = await _employeeService.UpdateEmployeeAsync(employeeId, updateModel);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("Jane");
        result.LastName.Should().Be("Smith");
        result.Email.Should().Be("new@example.com");
        result.PhoneNumber.Should().Be("123456789");

        _employeeRepoMock.Verify(repo => repo.GetByIdAsync(employeeId), Times.Once);
        _employeeRepoMock.Verify(repo => repo.Update(It.Is<Employee>(e => 
            e.FirstName == "Jane" && 
            e.LastName == "Smith" && 
            e.Email == "new@example.com" &&
            e.PhoneNumber == "123456789")), Times.Once);
    }

    [Fact]
    public async Task DeleteEmployeeAsync_ShouldCallDeleteByIdAsync()
    {
        // Arrange
        var employeeId = Guid.NewGuid();

        _employeeRepoMock.Setup(repo => repo.DeleteByIdAsync(employeeId))
                         .Returns(Task.CompletedTask);

        // Act
        await _employeeService.DeleteEmployeeAsync(employeeId);

        // Assert
        _employeeRepoMock.Verify(repo => repo.DeleteByIdAsync(employeeId), Times.Once);
    }

    [Fact]
    public async Task CreateEmployee_ShouldCallAddAsync()
    {
        // Arrange
        var createEmployeeModel = new CreateEmployeeModel
        {
            FirstName = "Alice",
            LastName = "Johnson",
            Email = "alice@example.com",
            PhoneNumber = "123456789",
            Location = null
        };

        var locationId = Guid.NewGuid();

        _employeeRepoMock.Setup(repo => repo.AddAsync(It.IsAny<Employee>()))
                         .Returns(Task.CompletedTask);

        // Act
        await _employeeService.CreateEmployee(createEmployeeModel);

        // Assert
        _employeeRepoMock.Verify(repo => repo.AddAsync(It.Is<Employee>(e => 
            e.FirstName == "Alice" && 
            e.LastName == "Johnson" && 
            e.Email == "alice@example.com" &&
            e.PhoneNumber == "123456789")), Times.Once);
    }
    
  
}


using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Shared.Models;
using Shared.Services;
using UserService.Application.Models;
using UserService.Application.Repositories;
using UserService.Domain.entities;


namespace UserService.Tests;


    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<ILocationServiceClient> _locationClientMock;

        private readonly UserService.Application.Services.UserService _userService;

        public UserServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _locationClientMock = new Mock<ILocationServiceClient>();

            _userService = new UserService.Application.Services.UserService(_userRepoMock.Object, _locationClientMock.Object );
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedUser = new User { Id = userId, FirstName = "John", LastName = "Doe" };

            _userRepoMock.Setup(repo => repo.GetByIdAsync(userId))
                         .ReturnsAsync(expectedUser);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(userId);
            result.FirstName.Should().Be("John");
            result.LastName.Should().Be("Doe");

            _userRepoMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateAndReturnUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User { Id = userId, FirstName = "John", LastName = "Doe", Email = "old@example.com" };
            var updateModel = new UpdateUserModel { FirstName = "Jane", LastName = "Smith", Email = "new@example.com", PhoneNumber = "123456789" };

            _userRepoMock.Setup(repo => repo.GetByIdAsync(userId))
                         .ReturnsAsync(existingUser);
            _userRepoMock.Setup(repo => repo.Update(It.IsAny<User>()))
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.UpdateUserAsync(userId, updateModel);

            // Assert
            result.Should().NotBeNull();
            result.FirstName.Should().Be("Jane");
            result.LastName.Should().Be("Smith");
            result.Email.Should().Be("new@example.com");
            result.PhoneNumber.Should().Be("123456789");

            _userRepoMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
            _userRepoMock.Verify(repo => repo.Update(It.Is<User>(u => 
                u.FirstName == "Jane" && 
                u.LastName == "Smith" && 
                u.Email == "new@example.com" &&
                u.PhoneNumber == "123456789")), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldCallDeleteByIdAsync()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userRepoMock.Setup(repo => repo.DeleteByIdAsync(userId))
                         .Returns(Task.CompletedTask);

            // Act
            await _userService.DeleteUserAsync(userId);

            // Assert
            _userRepoMock.Verify(repo => repo.DeleteByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task CreateUser_ShouldCallAddAsync()
        {
            // Arrange
            var createUserModel = new CreateUserModel
            {
                FirstName = "Alice",
                LastName = "Johnson",
                Email = "alice@example.com",
                PhoneNumber = "123456789",
                Location = null
            };

            var locationId = Guid.NewGuid();

            _userRepoMock.Setup(repo => repo.AddAsync(It.IsAny<User>()))
                         .Returns(Task.CompletedTask);

            // Act
            await _userService.CreateUser(createUserModel);

            // Assert
            _userRepoMock.Verify(repo => repo.AddAsync(It.Is<User>(u => 
                u.FirstName == "Alice" && 
                u.LastName == "Johnson" && 
                u.Email == "alice@example.com" &&
                u.PhoneNumber == "123456789")), Times.Once);
        }
        
        [Fact]
        public async Task CreateUser_ShouldContinue_IfLocationServiceIsDown()
        {
            // Arrange
            var createUserModel = new CreateUserModel
            {
                FirstName = "Alice",
                LastName = "Johnson",
                Email = "alice@example.com",
                PhoneNumber = "123456789",
                Location = new CreateLocationRequestModel(){City = "New York", Country = "USA", AddressLine = "New York, USA", PostalCode = "12345"}
            };

            _locationClientMock.Setup(repo => repo.CreateLocationAsync(It.IsAny<CreateLocationRequestModel>()))
                .Throws(new Exception());
            
            _userRepoMock.Setup(repo => repo.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            await _userService.CreateUser(createUserModel);

            // Assert - repo should still be called and locationId set to null 
            _userRepoMock.Verify(repo => repo.AddAsync(It.Is<User>(u => 
                u.FirstName == "Alice" && 
                u.LastName == "Johnson" && 
                u.Email == "alice@example.com" &&
                u.PhoneNumber == "123456789" && 
                u.LocationId == null)), Times.Once);
        }
    }



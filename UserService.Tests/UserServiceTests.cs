
using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Shared.Messaging;
using Shared.Messaging.Events.User;
using Shared.Messaging.Topics;
using Shared.Models;
using Shared.Services;
using UserService.Application.Interfaces;
using UserService.Application.Models;
using UserService.Domain.Repositories;
using UserService.Domain.entities;


namespace UserService.Tests;


    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IUserEventPublisher> _userEventPublisherMock;

        private readonly UserService.Application.Services.UserService _userService;

        public UserServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _userEventPublisherMock = new Mock<IUserEventPublisher>();

            _userService = new UserService.Application.Services.UserService(_userRepoMock.Object,  _userEventPublisherMock.Object);
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
            var result = await _userService.GetUserByIdAsync(userId, CancellationToken.None);

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
            var result = await _userService.UpdateUserAsync(userId, updateModel, CancellationToken.None);

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
            await _userService.DeleteUserAsync(userId, CancellationToken.None);

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


            _userRepoMock.Setup(repo => repo.AddAsync(It.IsAny<User>()))
                         .Returns(Task.CompletedTask);

            // Act
            await _userService.CreateUser(createUserModel, CancellationToken.None);

            // Assert
            _userRepoMock.Verify(repo => repo.AddAsync(It.Is<User>(u => 
                u.FirstName == "Alice" && 
                u.LastName == "Johnson" && 
                u.Email == "alice@example.com" &&
                u.PhoneNumber == "123456789")), Times.Once);
        }
        
        [Fact]
        public async Task CreateUser_Should_Call_AddAsync_And_PublishEvent()
        {
            // Arrange
            var userModel = new CreateUserModel
            {
                PhoneNumber = "123456789",
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Location = { }
            };

            _userRepoMock
                .Setup(repo => repo.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            _userEventPublisherMock
                .Setup(pub => pub.PublishUserCreatedAsync(It.IsAny<User>(), It.IsAny<CreateLocationRequestModel>()))
                .Returns(Task.CompletedTask);

            // Act
            await _userService.CreateUser(userModel, CancellationToken.None);

            // Assert
            _userRepoMock.Verify(repo => repo.AddAsync(It.Is<User>(u =>
                u.Email == userModel.Email &&
                u.PhoneNumber == userModel.PhoneNumber &&
                u.FirstName == userModel.FirstName &&
                u.LastName == userModel.LastName &&
                u.LocationId == null)), Times.Once);

            _userEventPublisherMock.Verify(pub => pub.PublishUserCreatedAsync(It.Is<User>(u =>
                u.Email == userModel.Email &&
                u.PhoneNumber == userModel.PhoneNumber &&
                u.FirstName == userModel.FirstName &&
                u.LastName == userModel.LastName
             ), It.IsAny<CreateLocationRequestModel>()), Times.Once);
        }
        
        [Fact]
    public async Task UpdateUserAsync_ValidData_UpdatesUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userModel = new UpdateUserModel
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PhoneNumber = "1234567890"
        };
        var existingUser = new User
        {
            Id = userId,
            FirstName = "OldName",
            LastName = "OldLastName",
            Email = "old.email@example.com",
            PhoneNumber = "9876543210"
        };

        _userRepoMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(existingUser);
        _userRepoMock.Setup(repo => repo.Update(It.IsAny<User>())).Returns(Task.CompletedTask);
        _userRepoMock.Setup(repo => repo.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((User)null); // No user with this email
        _userRepoMock.Setup(repo => repo.GetByPhoneAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((User)null); // No user with this phone number

        // Act
        var updatedUser = await _userService.UpdateUserAsync(userId, userModel, CancellationToken.None);

        // Assert
        Assert.NotNull(updatedUser);
        Assert.Equal(userModel.FirstName, updatedUser.FirstName);
        Assert.Equal(userModel.LastName, updatedUser.LastName);
        Assert.Equal(userModel.Email, updatedUser.Email);
        Assert.Equal(userModel.PhoneNumber, updatedUser.PhoneNumber);

        _userRepoMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Once);
    }

    

    [Fact]
    public async Task UpdateUserAsync_EmailAlreadyInUse_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userModel = new UpdateUserModel
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PhoneNumber = "1234567890"
        };

        var existingUser = new User
        {
            Id = userId,
            FirstName = "OldName",
            LastName = "OldLastName",
            Email = "old.email@example.com",
            PhoneNumber = "9876543210"
        };

        _userRepoMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(existingUser);
        _userRepoMock.Setup(repo => repo.Update(It.IsAny<User>())).Returns(Task.CompletedTask);
        _userRepoMock.Setup(repo => repo.GetByEmailAsync(userModel.Email, It.IsAny<CancellationToken>())).ReturnsAsync(existingUser); // Email already in use

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _userService.UpdateUserAsync(userId, userModel, CancellationToken.None)
        );
    }

    [Fact]
    public async Task UpdateUserAsync_PhoneNumberAlreadyInUse_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userModel = new UpdateUserModel
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PhoneNumber = "1234567890"
        };

        var existingUser = new User
        {
            Id = userId,
            FirstName = "OldName",
            LastName = "OldLastName",
            Email = "old.email@example.com",
            PhoneNumber = "9876543210"
        };

        _userRepoMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(existingUser);
        _userRepoMock.Setup(repo => repo.Update(It.IsAny<User>())).Returns(Task.CompletedTask);
        _userRepoMock.Setup(repo => repo.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((User)null); // No user with this email
        _userRepoMock.Setup(repo => repo.GetByPhoneAsync(userModel.PhoneNumber, It.IsAny<CancellationToken>())).ReturnsAsync(existingUser); // Phone already in use

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _userService.UpdateUserAsync(userId, userModel, CancellationToken.None)
        );
    }
        
    
    }



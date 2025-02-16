using LocationService.Application.Interfaces;
using LocationService.Application.Interfaces.Repositories;
using LocationService.Application.Messaging;
using LocationService.Application.Messaging.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shared.Messaging;
using Shared.Messaging.Events.Location;
using Shared.Messaging.Events.User;
using Shared.Messaging.Topics;
using Shared.Models;
using TrackPak.IntegrationTests.Utils;
using UserService.Application.Messaging.Handlers;
using UserService.Application.Models;
using UserService.Domain.entities;
using UserService.Domain.Repositories;
using UserService.Infrastructure.Messaging;

namespace TrackPak.IntegrationTests;


using System;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using Xunit;

public class UserServiceIntegrationTests : IClassFixture<RabbitMqTestContainer>
{
    private readonly IServiceProvider _serviceProvider;

    private readonly IMessageBus _rabbitMqServiceBus;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<ILocationRepository> _locationRepoMock;
    private readonly Mock<IGeocodingService> _geocodingServiceMock;
    private readonly UserEventPublisher _userEventPublisher;
    private readonly LocationEventPublisher _locationEventPublisher;
    private readonly UserCreatedHandler _userCreatedHandler;
    private readonly UserLocationCreatedHandler _userLocationCreatedHandler;
    private readonly UserLocationUpdatedHandler _userLocationUpdatedHandler;

    private readonly UserService.Application.Services.UserService _userService;
    private readonly LocationService.Application.Services.LocationService _locationService;

    public UserServiceIntegrationTests(RabbitMqTestContainer rabbitMqContainer)
    {
        // Create RabbitMQ connection using EasyNetQ
        // Create a service collection and configure EasyNetQ with RabbitMQ connection
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddEasyNetQ(rabbitMqContainer.ConnectionString);
        serviceCollection.AddSingleton<IMessageBus, RabbitMqServiceBus>();

        _serviceProvider = serviceCollection.BuildServiceProvider();
        _rabbitMqServiceBus = _serviceProvider.GetRequiredService<IMessageBus>();

        // Mock repositories
        _userRepoMock = new Mock<IUserRepository>();
        _locationRepoMock = new Mock<ILocationRepository>();
        _geocodingServiceMock = new Mock<IGeocodingService>();
        
        _userEventPublisher = new UserEventPublisher(_rabbitMqServiceBus);
        _locationEventPublisher = new LocationEventPublisher(_rabbitMqServiceBus);
        

        // Instantiate services with mocked repositories
        _userService = new UserService.Application.Services.UserService(_userRepoMock.Object, _userEventPublisher);
        _locationService = new LocationService.Application.Services.LocationService(_locationRepoMock.Object, _geocodingServiceMock.Object);

        _userCreatedHandler = new UserCreatedHandler(_locationService, _locationEventPublisher);
        _userLocationUpdatedHandler = new UserLocationUpdatedHandler(_locationService, _locationEventPublisher );
        _userLocationCreatedHandler = new UserLocationCreatedHandler(_userRepoMock.Object);

        // Subscribe handlers to RabbitMQ events
        _rabbitMqServiceBus.SubscribeAsync<UserCreatedEvent>(
            MessageTopic.UserCreated,
            async evt => await _userCreatedHandler.HandleAsync(evt, CancellationToken.None)
        );

        _rabbitMqServiceBus.SubscribeAsync<UserLocationCreatedEvent>(
            MessageTopic.UserLocationCreated,
            async evt => await _userLocationCreatedHandler.HandleAsync(evt, CancellationToken.None)
        );
        
        _rabbitMqServiceBus.SubscribeAsync<UserLocationUpdatedEvent>(
            MessageTopic.UserLocationCreated,
            async evt => await _userLocationUpdatedHandler.HandleAsync(evt, CancellationToken.None)
        );
        
       
        // Setup mock data
        var testUser = new User
        {
            Email = "test@example.com",
            PhoneNumber = "123456789",
            FirstName = "John",
            LastName = "Doe",
            LocationId = null // Initially null, will be updated
        };

        _userRepoMock.Setup(repo => repo.GetOrDefaultByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(testUser);
    }

    [Fact]
    public async Task CreateUser_Should_CreateUser_And_UpdateUser_With_LocationId()
    {
        // Arrange
        
        var userModel = new CreateUserModel
        {
            Email = "test@example.com",
            PhoneNumber = "123456789",
            FirstName = "John",
            LastName = "Doe",
            Location = new CreateLocationRequestModel(){Country = "US", City = "New York", PostalCode = "12345", AddressLine = "Street 12"}
        };
        
        User? updatedUser = null;

        _userRepoMock.Setup(repo => repo.Update(It.IsAny<User>()))
            .Callback<User>(user =>
            {
                updatedUser = user;
            })
            .Returns(Task.CompletedTask);
        
        // Act: Call UserService to create a user (this should trigger events)
        await _userService.CreateUser(userModel, CancellationToken.None);

        // Wait for LocationService to process event and publish UserLocationCreatedEvent
        await Task.Delay(2000); 
        

        // Assert: Verify that UserService created and updated the user's location (hence UserCreatedEvent and UserLocationCreatedEvent was processed)
        _userRepoMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
        _userRepoMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Once);
        
        Assert.NotNull(updatedUser);
        Assert.NotNull(updatedUser!.LocationId); 

    }
    
    [Fact]
    public async Task UpdateUserLocation_Should_CreateNewLocation_And_UpdateUser_With_LocationId()
    {
        // Arrange

        var locationModel = new CreateLocationRequestModel()
            {Country = "US", City = "New York", PostalCode = "12345", AddressLine = "Street 12"};
        
        
        Guid userId = Guid.NewGuid();
        User? updatedUser = null;
        var testUser = new User
        {
            Id = userId,
            Email = "test@example.com",
            PhoneNumber = "123456789",
            FirstName = "John",
            LastName = "Doe",
            LocationId = null // Initially null, will be updated
        };

        _userRepoMock.Setup(repo => repo.ExistsAsync(It.IsAny<Guid>()))!
            .ReturnsAsync((Guid id) => true); 
        
        _userRepoMock.Setup(repo => repo.Update(It.IsAny<User>()))
            .Callback<User>(user =>
            {
                updatedUser = user;
            })
            .Returns(Task.CompletedTask);
        
        // Act: Call UserService to create a user (this should trigger events)
        await _userService.UpdateUserLocationAsync(userId, locationModel, CancellationToken.None);

        // Wait for LocationService to process event and publish UserLocationCreatedEvent
        await Task.Delay(2000); 
        

        // Assert: Verify that UserService created and updated the user's location (hence UserCreatedEvent and UserLocationCreatedEvent was processed)
        _userRepoMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Once);
        
        Assert.NotNull(updatedUser);
        Assert.NotNull(updatedUser!.LocationId); 

    }
}

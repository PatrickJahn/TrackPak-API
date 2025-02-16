using CompanyService.Application.Interfaces;
using CompanyService.Application.Messaging.Handlers;
using CompanyService.Application.Models;
using CompanyService.Domain.Entities;
using CompanyService.Domain.Interfaces;
using CompanyService.Infrastructure.Messaging;
using EasyNetQ;
using LocationService.Application.Interfaces;
using LocationService.Application.Interfaces.Repositories;
using LocationService.Application.Messaging;
using LocationService.Application.Messaging.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shared.Messaging;
using Shared.Messaging.Events.Company;
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

public class CompanyServiceIntegrationTests : IClassFixture<RabbitMqTestContainer>
{
    private readonly IServiceProvider _serviceProvider;

    private readonly IMessageBus _rabbitMqServiceBus;
    private readonly Mock<ICompanyRepository> _companyRepositoryMock;
    private readonly Mock<ILocationRepository> _locationRepoMock;
    private readonly Mock<IGeocodingService> _geocodingServiceMock;
    private readonly ICompanyEventPublisher _companyEventPublisher;
    private readonly LocationEventPublisher _locationEventPublisher;
    private readonly CompanyCreatedHandler _companyCreatedHandler;
    private readonly CompanyLocationCreatedHandler _companyLocationCreatedHandler;

    private readonly CompanyService.Application.Services.CompanyService _companyService;
    private readonly LocationService.Application.Services.LocationService _locationService;

    public CompanyServiceIntegrationTests(RabbitMqTestContainer rabbitMqContainer)
    {
        // Create RabbitMQ connection using EasyNetQ
        // Create a service collection and configure EasyNetQ with RabbitMQ connection
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddEasyNetQ(rabbitMqContainer.ConnectionString);
        serviceCollection.AddSingleton<IMessageBus, RabbitMqServiceBus>();

        _serviceProvider = serviceCollection.BuildServiceProvider();
        _rabbitMqServiceBus = _serviceProvider.GetRequiredService<IMessageBus>();

        // Mock repositories
        _companyRepositoryMock = new Mock<ICompanyRepository>();
        _locationRepoMock = new Mock<ILocationRepository>();
        _geocodingServiceMock = new Mock<IGeocodingService>();
        
        _companyEventPublisher = new CompanyEventPublisher(_rabbitMqServiceBus);
        _locationEventPublisher = new LocationEventPublisher(_rabbitMqServiceBus);
        

        // Instantiate services with mocked repositories
        _companyService = new CompanyService.Application.Services.CompanyService(_companyRepositoryMock.Object, _companyEventPublisher);
        _locationService = new LocationService.Application.Services.LocationService(_locationRepoMock.Object, _geocodingServiceMock.Object);

        _companyCreatedHandler = new CompanyCreatedHandler(_locationService, _locationEventPublisher);
        _companyLocationCreatedHandler = new CompanyLocationCreatedHandler(_companyRepositoryMock.Object);

        // Subscribe handlers to RabbitMQ events
        _rabbitMqServiceBus.SubscribeAsync<CompanyCreatedEvent>(
            MessageTopic.UserCreated,
            async evt => await _companyCreatedHandler.HandleAsync(evt, CancellationToken.None)
        );

        _rabbitMqServiceBus.SubscribeAsync<CompanyLocationCreatedEvent>(
            MessageTopic.UserLocationCreated,
            async evt => await _companyLocationCreatedHandler.HandleAsync(evt, CancellationToken.None)
        );
        
    
       
        // Setup mock data
        var testCompany = new Company()
        {
           Cvr = Guid.NewGuid().ToString(),
           Name = "Test Company",
           BrandId = Guid.NewGuid().ToString(),
            LocationId = null // Initially null, will be updated
        };

        _companyRepositoryMock.Setup(repo => repo.GetOrDefaultByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(testCompany);
    }
    
    
    [Fact]
    public async Task CreateCompany_Should_CreateCompany_And_Update_Company_With_LocationId()
    {
        // Arrange
        
        var companyModel = new CreateCompanyModel()
        {
          Cvr = Guid.NewGuid().ToString(), Name = "Test Company",
          BrandId = Guid.NewGuid().ToString(),
            Location = new CreateLocationRequestModel(){Country = "US", City = "New York", PostalCode = "12345", AddressLine = "Street 12"}
        };
        
        Company? updatedCompany = null;

        _companyRepositoryMock.Setup(repo => repo.Update(It.IsAny<Company>()))
            .Callback<Company>(company =>
            {
                updatedCompany = company;
            })
            .Returns(Task.CompletedTask);
        
        // Act: Call UserService to create a user (this should trigger events)
        await _companyService.CreateCompanyAsync(companyModel, CancellationToken.None);

        // Wait for LocationService to process event and publish UserLocationCreatedEvent
        await Task.Delay(2000); 
        

        // Assert: Verify that UserService created and updated the user's location (hence UserCreatedEvent and UserLocationCreatedEvent was processed)
        _companyRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Company>()), Times.Once);
        _companyRepositoryMock.Verify(repo => repo.Update(It.IsAny<Company>()), Times.Once);
        
        Assert.NotNull(updatedCompany);
        Assert.NotNull(updatedCompany!.LocationId); 

    }
    
   
}

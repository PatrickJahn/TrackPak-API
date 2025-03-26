using FluentAssertions;
using Moq;
using OrderService.Application.Interfaces;
using OrderService.Application.Models;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Domain.Interfaces;
using Shared.Models;

namespace OrderService.Tests;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IOrderEventPublisher> _eventPublisherMock;
    private readonly OrderService.Application.Services.OrderService _sut;

    public OrderServiceTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _eventPublisherMock = new Mock<IOrderEventPublisher>();
        _sut = new OrderService.Application.Services.OrderService(_orderRepositoryMock.Object, _eventPublisherMock.Object);
    }

    [Fact]
    public async Task CreateOrderAsync_Should_Create_And_Publish_Order()
    {
        // Arrange
        var companyId = Guid.NewGuid();

        var createModel = new CreateOrderModel
        {
            Type = OrderType.Service,
            CompanyId = companyId,
            Description = "Install fiber line",
            Location = new CreateLocationRequestModel
            {
                Country = "Denmark",
                City = "Copenhagen",
                AddressLine = "Tech Street 5",
                PostalCode = "2100"
            },
            User = new CreateUserRequestModel
            {
                FirstName = "Alice",
                LastName = "Johnson",
                Email = "alice@example.com",
                PhoneNumber = "12345678"
            },
            OrderItems = new List<OrderItemDto>
            {
                new() { Title = "Modem", Price = 499.99m, Quantity = 1 },
                new() { Title = "Router", Price = 299.50m, Quantity = 2 }
            }
        };

        Order? capturedOrder = null;

        _orderRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Order>()))
            .Callback<Order>(order => capturedOrder = order)
            .Returns(Task.CompletedTask);

        _eventPublisherMock
            .Setup(pub => pub.PublishOrderCreatedAsync(It.IsAny<Order>(), createModel.Location, createModel.User))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateOrderAsync(createModel);

        // Assert
        result.Should().NotBeNull();
        capturedOrder.Should().NotBeNull();

        capturedOrder!.CompanyId.Should().Be(companyId);
        capturedOrder.Type.Should().Be(OrderType.Service);
        capturedOrder.Description.Should().Be("Install fiber line");

        capturedOrder.OrderItems.Should().HaveCount(2);
        var items = capturedOrder.OrderItems.ToList();
        items[0].Title.Should().Be("Modem");
        items[1].Price.Should().Be(299.50m);
        items[1].Quantity.Should().Be(2);


        _orderRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Order>()), Times.Once);

        _eventPublisherMock.Verify(pub =>
                pub.PublishOrderCreatedAsync(
                    It.Is<Order>(o => o == capturedOrder),
                    createModel.Location,
                    createModel.User),
            Times.Once);
    }
}
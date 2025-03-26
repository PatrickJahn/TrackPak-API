
using Moq;
using RouteService.Domain.Entities;
using RouteService.Domain.Interfaces;
using Shared.Enums;
using Shared.Messaging.Events.Order;

namespace RouteService.Tests;

public class RouteServiceTests
{
    private readonly Mock<IRouteRepository> _routeRepositoryMock;
    private readonly Mock<IRouteOptimizer> _routeOptimizerMock;
    private readonly RouteService.Application.Services.RouteService _sut;

    public RouteServiceTests()
    {
        _routeRepositoryMock = new Mock<IRouteRepository>();
        _routeOptimizerMock = new Mock<IRouteOptimizer>();
        _sut = new RouteService.Application.Services.RouteService(_routeRepositoryMock.Object, _routeOptimizerMock.Object);
    }

    [Fact]
    public async Task HandleOrderCreatedAsync_Should_Create_New_Unassigned_Route_If_None_Exist()
    {
        // Arrange
        var orderEvent = new OrderCreatedEvent { OrderId = Guid.NewGuid() };

        _routeRepositoryMock
            .Setup(r => r.GetRoutesByEmployeeIdAsync(null))
            .ReturnsAsync(new List<Route>());

        // Act
        await _sut.HandleOrderCreatedAsync(orderEvent);

        // Assert
        _routeRepositoryMock.Verify(r => r.CreateRouteAsync(It.Is<Route>(route =>
            route.EmployeeId == null &&
            route.Status == RouteStatusEnum.Pending &&
            route.OrderRoutes.Count == 1 &&
            route.OrderRoutes.First().OrderId == orderEvent.OrderId &&
            route.OrderRoutes.First().Sequence == 1
        )), Times.Once);

        _routeRepositoryMock.Verify(r => r.UpdateRouteAsync(It.IsAny<Route>()), Times.Never);
    }

    [Fact]
    public async Task HandleOrderCreatedAsync_Should_Append_To_Existing_Route_If_Exists()
    {
        // Arrange
        var existingRoute = new Route
        {
            EmployeeId = null,
            OrderRoutes = new List<OrderRoute>
            {
                new OrderRoute { OrderId = Guid.NewGuid(), Sequence = 1 }
            },
            Status = RouteStatusEnum.Pending
        };

        var orderEvent = new OrderCreatedEvent { OrderId = Guid.NewGuid() };

        _routeRepositoryMock
            .Setup(r => r.GetRoutesByEmployeeIdAsync(null))
            .ReturnsAsync(new List<Route> { existingRoute });

        // Act
        await _sut.HandleOrderCreatedAsync(orderEvent);

        // Assert
        _routeRepositoryMock.Verify(r => r.UpdateRouteAsync(It.Is<Route>(route =>
            route.OrderRoutes.Count == 2 &&
            route.OrderRoutes.Any(o => o.OrderId == orderEvent.OrderId) &&
            route.OrderRoutes.First(o => o.OrderId == orderEvent.OrderId).Sequence == 2
        )), Times.Once);

        _routeRepositoryMock.Verify(r => r.CreateRouteAsync(It.IsAny<Route>()), Times.Never);
    }
}

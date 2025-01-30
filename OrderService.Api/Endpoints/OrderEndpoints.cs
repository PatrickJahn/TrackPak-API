using Microsoft.AspNetCore.Http.HttpResults;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

namespace OrderService.Api.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/orders");

        group.MapPost("/", CreateOrderAsync);
        group.MapGet("/{orderId:guid}", GetOrderByIdAsync);
        group.MapGet("/", GetOrdersAsync);
        group.MapPut("/{orderId:guid}", UpdateOrderAsync);
        group.MapDelete("/{orderId:guid}", DeleteOrderAsync);
        group.MapPatch("/{orderId:guid}/status", UpdateOrderStatusAsync);
        group.MapPost("/{orderId:guid}/cancel", CancelOrderAsync);
    }

    private static async Task<Results<Created<Order>, BadRequest<string>>> CreateOrderAsync(
        Order order,
        IOrderService orderService,
        CancellationToken cancellationToken)
    {
        var createdOrder = await orderService.CreateOrderAsync(order, cancellationToken);
        return createdOrder != null ? TypedResults.Created($"/orders/{createdOrder.Id}", createdOrder) 
                                    : TypedResults.BadRequest("Failed to create order.");
    }

    private static async Task<Results<Ok<Order>, NotFound>> GetOrderByIdAsync(
        Guid orderId,
        IOrderService orderService,
        CancellationToken cancellationToken)
    {
        var order = await orderService.GetOrderByIdAsync(orderId, cancellationToken);
        return order != null ? TypedResults.Ok(order) : TypedResults.NotFound();
    }

    private static async Task<Ok<IEnumerable<Order>>> GetOrdersAsync(
        Guid? userId,
        Guid? companyId,
        OrderStatus? status,
        IOrderService orderService,
        CancellationToken cancellationToken)
    {
        var orders = await orderService.GetOrdersAsync(userId, companyId, status, cancellationToken);
        return TypedResults.Ok(orders);
    }

    private static async Task<Results<NoContent, NotFound>> UpdateOrderAsync(
        Guid orderId,
        Order updatedOrder,
        IOrderService orderService,
        CancellationToken cancellationToken)
    {
        var updated = await orderService.UpdateOrderAsync(orderId, updatedOrder, cancellationToken);
        return updated ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<NoContent, NotFound>> DeleteOrderAsync(
        Guid orderId,
        IOrderService orderService,
        CancellationToken cancellationToken)
    {
        var deleted = await orderService.DeleteOrderAsync(orderId, cancellationToken);
        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<NoContent, NotFound>> UpdateOrderStatusAsync(
        Guid orderId,
        OrderStatus newStatus,
        IOrderService orderService,
        CancellationToken cancellationToken)
    {
        var updated = await orderService.UpdateOrderStatusAsync(orderId, newStatus, cancellationToken);
        return updated ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<NoContent, NotFound>> CancelOrderAsync(
        Guid orderId,
        IOrderService orderService,
        CancellationToken cancellationToken)
    {
        var canceled = await orderService.CancelOrderAsync(orderId, cancellationToken);
        return canceled ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}

using Microsoft.AspNetCore.Http.HttpResults;
using OrderService.Application.Interfaces;
using OrderService.Application.Models;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using Shared.Extensions;
using Shared.Security;

namespace OrderService.Api.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
       {
           var group = app.MapGroup("/orders");
   
           // Requires either CompanyAdmin or SystemAdmin to create orders
           group.MapPost("/", CreateOrderAsync)
               .RequireAuthorization(PolicyRoles.CompanyAdmin);
   
           // SystemAdmin or CompanyAdmin can read any order
           group.MapGet("/{orderId:guid}", GetOrderByIdAsync)
               .RequireAuthorization(PolicyRoles.CompanyAdmin);
   
           // CompanyAdmin can view all orders
           group.MapGet("/", GetOrdersAsync)
               .RequireAuthorization(PolicyRoles.CompanyAdmin);
   
           // Customers can view their own orders
           group.MapGet("/my-orders", GetMyOrdersAsync)
               .RequireAuthorization(PolicyRoles.Customer);
   
           // Updating an order is restricted to CompanyAdmin or SystemAdmin
           group.MapPut("/{orderId:guid}", UpdateOrderAsync)
               .RequireAuthorization(PolicyRoles.CompanyAdmin);
   
           // Deleting an order is restricted to SystemAdmin only
           group.MapDelete("/{orderId:guid}", DeleteOrderAsync)
               .RequireAuthorization(PolicyRoles.SystemAdmin);
   
           // Updating order status is restricted to CompanyAdmin or SystemAdmin
           group.MapPatch("/{orderId:guid}/status", UpdateOrderStatusAsync)
               .RequireAuthorization(PolicyRoles.CompanyAdmin);
   
           // Canceling order is allowed for Customers
           group.MapPost("/{orderId:guid}/cancel", CancelOrderAsync)
               .RequireAuthorization(PolicyRoles.Customer);
       }

    private static async Task<Results<Created<Order>, BadRequest<string>>> CreateOrderAsync(
        CreateOrderModel order,
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
    private static async Task<Ok<IEnumerable<Order>>> GetMyOrdersAsync(
        HttpContext httpContext,
        IOrderService orderService,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.GetUserId() ?? Guid.Empty;
        var orders = await orderService.GetMyOrdersAsync(userId, cancellationToken);
        return TypedResults.Ok(orders);
    }
    private static async Task<Results<NoContent, NotFound>> UpdateOrderAsync(
        Guid orderId,
        Order updatedOrder,
        IOrderService orderService,
        CancellationToken cancellationToken)
    {
        // TODO: Create dto request for updateOrder
        
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

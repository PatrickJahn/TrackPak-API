using OrderService.Domain.Interfaces;
using Shared.Messaging;
using Shared.Messaging.Events.User;

namespace OrderService.Application.Messaging.Handlers;

public class OrderUserCreatedEventHandler(IOrderRepository orderRepository) : IMessageHandler<OrderUserCreatedEvent>
{
    public async Task HandleAsync(OrderUserCreatedEvent message, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdAsync(message.OrderId);
        if (order != null)
        {
            order.UserId = message.UserId;
            await orderRepository.Update(order);
            Console.WriteLine($"✅ UserId {message.UserId} linked to Order {message.OrderId}");
        }
        else
        {
            Console.WriteLine($"⚠️ Order {message.OrderId} not found for User assignment.");
        }
    }
}
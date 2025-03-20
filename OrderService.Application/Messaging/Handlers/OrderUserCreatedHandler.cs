

using OrderService.Domain.Interfaces;
using Shared.Messaging;
using Shared.Messaging.Events.Location;
using Shared.Messaging.Events.User;

namespace OrderService.Application.Messaging.Handlers;


public class OrderUserCreatedHandler(IOrderRepository orderRepository)
    : IMessageHandler<OrderUserCreatedEvent>
{
    public async Task HandleAsync(OrderUserCreatedEvent message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Updating order {message.OrderId} with userId {message.UserId}");

        
        var order = await orderRepository.GetOrDefaultByIdAsync(message.OrderId);

        if (order == null)
        {
            // TODO: Add logic 
            return;
        }

        order.UserId = message.UserId;
        await orderRepository.Update(order);
    }
}

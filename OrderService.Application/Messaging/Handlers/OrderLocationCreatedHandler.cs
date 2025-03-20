using OrderService.Domain.Interfaces;
using Shared.Messaging;
using Shared.Messaging.Events.Location;

namespace OrderService.Application.Messaging.Handlers;


public class OrderLocationCreatedHandler(IOrderRepository orderRepository)
    : IMessageHandler<OrderLocationCreatedEvent>
{
    public async Task HandleAsync(OrderLocationCreatedEvent message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Updating order {message.OrderId} with location {message.LocationId}");

        
        var order = await orderRepository.GetOrDefaultByIdAsync(message.OrderId);

        if (order == null)
        {
            // TODO: Add logic 
            return;
        }

        order.LocationId = message.LocationId;
        await orderRepository.Update(order);
    }
}

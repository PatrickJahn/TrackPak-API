using LocationService.Application.Interfaces;
using Shared.Messaging;
using Shared.Messaging.Events.Location;
using Shared.Messaging.Events.Order;
using Shared.Messaging.Events.User;

namespace LocationService.Application.Messaging.Handlers;

public class OrderCreatedHandler(ILocationService locationService, ILocationEventPublisher locationEventPublisher)  : IMessageHandler<OrderCreatedEvent>
{
    
    public async Task HandleAsync(OrderCreatedEvent message, CancellationToken cancellationToken)
    {
        // TODO: implement SAGA Pattern
        var location =  await locationService.CreateLocationAsync(message.Location);

        await locationEventPublisher.PublishOrderLocationCreatedAsync(new OrderLocationCreatedEvent()
        {
            LocationId = location.Id,
            OrderId = message.OrderId
        });
    }
}
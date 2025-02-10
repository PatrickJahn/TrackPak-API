using LocationService.Application.Interfaces;
using Shared.Messaging;
using Shared.Messaging.Events.Employee;
using Shared.Messaging.Events.Location;
using Shared.Messaging.Events.Order;
using Shared.Messaging.Events.User;

namespace LocationService.Application.Messaging.Handlers;

public class EmployeeCreatedHandler(ILocationService locationService, ILocationEventPublisher locationEventPublisher)  : IMessageHandler<EmployeeCreatedEvent>
{
    

    public async Task HandleAsync(EmployeeCreatedEvent message, CancellationToken cancellationToken)
    {
        // TODO: implement SAGA Pattern
        var location =  await locationService.CreateLocationAsync(message.Location);

        await locationEventPublisher.PublishEmployeeLocationCreatedAsync(new EmployeeLocationCreatedEvent()
        {
            LocationId = location.Id,
            EmployeeId = message.EmployeeId
        });
        
    }
}
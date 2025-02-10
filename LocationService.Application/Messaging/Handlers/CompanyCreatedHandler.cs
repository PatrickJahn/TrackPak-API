using LocationService.Application.Interfaces;
using Shared.Messaging;
using Shared.Messaging.Events.Company;
using Shared.Messaging.Events.Location;
using Shared.Messaging.Events.Order;

namespace LocationService.Application.Messaging.Handlers;

public class CompanyCreatedHandler(ILocationService locationService, ILocationEventPublisher locationEventPublisher)  : IMessageHandler<CompanyCreatedEvent>
{
    public async Task HandleAsync(CompanyCreatedEvent message, CancellationToken cancellationToken)
    {
        // TODO: implement SAGA Pattern
            var location =  await locationService.CreateLocationAsync(message.Location);
    
            await locationEventPublisher.PublishCompanyLocationCreatedAsync(new CompanyLocationCreatedEvent()
            {
                LocationId = location.Id,
                CompanyId = message.CompanyId
            });
    }
}
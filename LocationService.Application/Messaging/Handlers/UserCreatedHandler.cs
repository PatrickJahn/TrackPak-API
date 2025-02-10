using LocationService.Application.Interfaces;
using Shared.Messaging;
using Shared.Messaging.Events.Location;
using Shared.Messaging.Events.User;

namespace LocationService.Application.Messaging.Handlers;

public class UserCreatedHandler(ILocationService locationService, ILocationEventPublisher locationEventPublisher)  : IMessageHandler<UserCreatedEvent>
{
    public async Task HandleAsync(UserCreatedEvent message, CancellationToken cancellationToken)
    {
        // TODO: implement SAGA Pattern
      var location =  await locationService.CreateLocationAsync(message.Location);

      await locationEventPublisher.PublishUserLocationCreatedAsync(new UserLocationCreatedEvent()
      {
          LocationId = location.Id,
          UserId = message.UserId
      });
    }
}
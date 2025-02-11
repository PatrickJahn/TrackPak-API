using Shared.Messaging;
using Shared.Messaging.Events.User;
using Shared.Messaging.Topics;
using Shared.Models;
using UserService.Application.Interfaces;
using UserService.Application.Models;

namespace UserService.Infrastructure.Messaging;

public class UserEventPublisher(IMessageBus messageBus) : IUserEventPublisher
{
    public async Task PublishUserCreatedAsync(CreateUserModel user)
    {
        var userCreatedEvent = new UserCreatedEvent
        {
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Location = user.Location
        };

        await messageBus.PublishAsync(MessageTopic.UserCreated, userCreatedEvent);
    }

    public async Task PublishUserLocationUpdatedAsync(Guid userId, UpdateLocationModel model)
    {

        var userLocationUpdatedEvent = new UserLocationUpdatedEvent()
        {
            Location = model,
            UserId = userId
        };
        await messageBus.PublishAsync(MessageTopic.UserLocationUpdated, userLocationUpdatedEvent);
    }
}
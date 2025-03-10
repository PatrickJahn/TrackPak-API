using Shared.Messaging;
using Shared.Messaging.Events.User;
using Shared.Messaging.Topics;
using Shared.Models;
using UserService.Application.Interfaces;
using UserService.Application.Models;
using UserService.Domain.entities;

namespace UserService.Infrastructure.Messaging;

public class UserEventPublisher(IMessageBus messageBus) : IUserEventPublisher
{
    public async Task PublishUserCreatedAsync(User user, CreateLocationRequestModel location)
    {
        var userCreatedEvent = new UserCreatedEvent
        {
            UserId = user.Id,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Location = location
        };

        await messageBus.PublishAsync(MessageTopic.UserCreated, userCreatedEvent);
    }

    public async Task PublishUserLocationUpdatedAsync(Guid userId, CreateLocationRequestModel model)
    {

        var userLocationUpdatedEvent = new UserLocationUpdatedEvent()
        {
            Location = model,
            UserId = userId
        };
        await messageBus.PublishAsync(MessageTopic.UserLocationUpdated, userLocationUpdatedEvent);
    }
}
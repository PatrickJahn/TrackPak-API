using Shared.Messaging;
using Shared.Messaging.Events.User;
using Shared.Messaging.Topics;
using UserService.Application.Interfaces;
using UserService.Application.Models;

namespace UserService.Application.Messaging;

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
}
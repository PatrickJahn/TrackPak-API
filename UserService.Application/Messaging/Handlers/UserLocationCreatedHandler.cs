using Shared.Messaging;
using Shared.Messaging.Events.Location;
using UserService.Application.Interfaces;
using UserService.Domain.Repositories;

namespace UserService.Application.Messaging.Handlers;

public class UserLocationCreatedHandler(IUserRepository userRepository) : IMessageHandler<UserLocationCreatedEvent>
{
    public async Task HandleAsync(UserLocationCreatedEvent message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Updating user {message.UserId} with location {message.LocationId}");

        var user = await userRepository.GetOrDefaultByIdAsync(message.UserId);
      
        if (user == null)
        {
            // TODO: Add logic 
            return;
        }
        
        user.LocationId = message.LocationId;
        await userRepository.Update(user);
    }
}
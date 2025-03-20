using Shared.Messaging;
using Shared.Messaging.Events.Location;
using Shared.Messaging.Events.Order;
using UserService.Application.Interfaces;
using UserService.Application.Models;

namespace UserService.Application.Messaging.Handlers;



public class OrderCreatedHandler(IUserService userService, IUserEventPublisher userEventPublisher)  : IMessageHandler<OrderCreatedEvent>
{
    
    public async Task HandleAsync(OrderCreatedEvent message, CancellationToken cancellationToken)
    {
        // TODO: implement SAGA Pattern

        if (message.User == null) 
            return;
        
        // TODO: cre
        var user = new CreateUserModel()
        {
            Email = message.User.Email,
            FirstName = message.User.FirstName,
            LastName = message.User.LastName,
            PhoneNumber = message.User.PhoneNumber ?? "",
            Location = message.Location
        };
        
        var userId =  await userService.CreateUser(user, cancellationToken);
        
        await userEventPublisher.PublishOrderUserCreatedAsync(userId, message.OrderId);
        
    }
}
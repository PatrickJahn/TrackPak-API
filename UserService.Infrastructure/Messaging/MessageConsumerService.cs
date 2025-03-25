using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Shared.Messaging;
using Shared.Messaging.Events.Location;
using Shared.Messaging.Events.Order;
using Shared.Messaging.Topics;
using UserService.Application.Interfaces;

namespace UserService.Infrastructure.Messaging;

public class MessageConsumerService(IMessageBus messageBus, IServiceProvider serviceProvider) 
    : BackgroundService
{
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
     await messageBus.SubscribeAsync<OrderCreatedEvent>(
         MessageTopic.OrderCreated, "UserService", async (message) =>
         {
             using var scope = serviceProvider.CreateScope();
             var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
             var publisher = scope.ServiceProvider.GetRequiredService<IUserEventPublisher>();
     
             //  Find user by email (from OrderCreatedEvent)
             var userId = await userService.GetUserByEmailAsync(message.User.Email, CancellationToken.None);
             if (userId != null)
             {
                 // Publish response back to OrderService with the userId
                 await publisher.PublishOrderUserCreatedAsync(userId, message.OrderId);
             }
         });
        await messageBus.SubscribeAsync<UserLocationCreatedEvent>(MessageTopic.UserLocationCreated, "UserService", async message =>
        {
            using var scope = serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<UserLocationCreatedEvent>>();

            try
            {
                Console.WriteLine($"UserLocationCreatedEvent received: LocationId: {message.LocationId}, UserId: {message.UserId}");
                await handler.HandleAsync(message, stoppingToken);
                
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling message: {ex.Message}");
            }
            
        });
        
        await Task.Delay(Timeout.Infinite, stoppingToken);

    }
}
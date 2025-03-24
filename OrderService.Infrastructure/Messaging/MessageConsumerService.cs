using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderService.Application.Interfaces;
using Shared.Messaging;
using Shared.Messaging.Events.Location;
using Shared.Messaging.Events.Order;
using Shared.Messaging.Events.User;
using Shared.Messaging.Topics;

namespace OrderService.Infrastructure.Messaging;

public class MessageConsumerService(IMessageBus messageBus, IServiceProvider serviceProvider) 
    : BackgroundService
{
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      
        await messageBus.SubscribeAsync<OrderLocationCreatedEvent>(MessageTopic.OrderLocationCreated,  "OrderService",  OrderLocationCreatedHandler);
        
        await messageBus.SubscribeAsync<OrderUserCreatedEvent>(
            MessageTopic.OrderUserCreated,  "OrderService", async (message) =>
            {
                using var scope = serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<OrderUserCreatedEvent>>();
                await handler.HandleAsync(message, stoppingToken);
            });

        
        
        async void OrderLocationCreatedHandler(OrderLocationCreatedEvent message)
        {
            using var scope = serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<OrderLocationCreatedEvent>>();

            try
            {
                Console.WriteLine($"OrderLocationCreatedEvent received: LocationId: {message.LocationId}, OrderId: {message.OrderId}");
                await handler.HandleAsync(message, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling message: {ex.Message}");
            }
        }
        await messageBus.SubscribeAsync<FetchCompanyOrdersEvent>(
            MessageTopic.FetchCompanyOrders, "OrderService", async (message) =>
            {
                using var scope = serviceProvider.CreateScope();
                var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<FetchCompanyOrdersEvent>>();
          
                var companyOrders = await orderService.GetOrdersAsync(null, message.CompanyId, null, System.Threading.CancellationToken.None);

                await handler.HandleAsync(message, stoppingToken);
                Console.WriteLine($"✅ Orders for company {message.CompanyId} published back to RouteService.");
            });
        

    }
}
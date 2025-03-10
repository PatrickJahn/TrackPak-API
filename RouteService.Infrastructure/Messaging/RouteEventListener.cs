using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RouteService.Application.Interfaces;
using Shared.Messaging;
using Shared.Messaging.Events.Order;
using Shared.Messaging.Topics;

namespace RouteService.Infrastructure.Messaging;

public class RouteEventListener(IServiceProvider serviceProvider, IMessageBus messageBus) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await messageBus.SubscribeAsync<OrderCreatedEvent>(MessageTopic.OrderCreated, async (orderEvent) =>
        {
            using var scope = serviceProvider.CreateScope();
            var routeService = scope.ServiceProvider.GetRequiredService<IRouteService>();
            await routeService.HandleOrderCreatedAsync(orderEvent);
        });

        await messageBus.SubscribeAsync<OrderCancelledEvent>(MessageTopic.OrderCancelled, async (orderEvent) =>
        {
            using var scope = serviceProvider.CreateScope();
            var routeService = scope.ServiceProvider.GetRequiredService<IRouteService>();
            await routeService.HandleOrderCancelledAsync(orderEvent);
        });
    }
}

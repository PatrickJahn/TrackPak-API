using RouteService.Application.Interfaces;
using RouteService.Domain.Entities;
using RouteService.Infrastructure.Messaging.Events;
using Shared.Enums;
using Shared.Messaging;
using Shared.Messaging.Events.Order;
using Shared.Messaging.Events.Route;
using Shared.Messaging.Topics;

namespace RouteService.Infrastructure.Messaging;

public class RouteEventPublisher(IMessageBus messageBus) : IRouteEventPublisher
{
    public async Task PublishRouteCreatedAsync(Route route)
    {
        var eventMessage = new RouteCreatedEvent
        {
            RouteId = route.Id,
            CompanyId = route.CompanyId,
            EmployeeId = route.EmployeeId,
            OrderRoutes = route.OrderRoutes
                .Select(o => new OrderRoute
                {
                    Sequence = o.Sequence,
                    OrderId = o.OrderId
                }).ToList(),
            EstimatedCompletionTime = route.CompletedAt
        };

        await messageBus.PublishAsync( MessageTopic.RouteCreated, eventMessage);
    }

    public async Task PublishRouteUpdatedAsync(Guid routeId, RouteStatusEnum status, DateTime? estimatedCompletionTime)
    {
        var eventMessage = new RouteUpdatedEvent
        {
            RouteId = routeId,
            Status = status,
            EstimatedCompletionTime = estimatedCompletionTime
        };

        await messageBus.PublishAsync( MessageTopic.RouteUpdated, eventMessage);
    }

    public async Task PublishRouteCompletedAsync(Route route)
    {
        var eventMessage = new RouteCompletedEvent
        {
            RouteId = route.Id,
            CompletedAt = route.CompletedAt ?? DateTime.UtcNow
        };

        await messageBus.PublishAsync( MessageTopic.RouteCompleted, eventMessage);
    }

    public async Task PublishRouteDelayedAsync(Guid routeId, TimeSpan delayDuration, string reason)
    {
        var eventMessage = new RouteDelayedEvent
        {
            RouteId = routeId,
            DelayDuration = delayDuration,
            Reason = reason
        };

        await messageBus.PublishAsync(MessageTopic.RouteDelayed,eventMessage);
    }

    public async Task PublishFetchCompanyOrdersAsync(Guid companyId)
    {
        var fetchEvent = new FetchCompanyOrdersEvent
        {
            CompanyId = companyId
        };
        await messageBus.PublishAsync(MessageTopic.FetchCompanyOrders, fetchEvent);
    }

}

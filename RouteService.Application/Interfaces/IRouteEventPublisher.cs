using RouteService.Domain.Entities;
using Shared.Enums;
using Shared.Models;

namespace RouteService.Application.Interfaces;

public interface IRouteEventPublisher
{
    /// <summary>
    /// Publishes an event when a new route is created.
    /// </summary>
    Task PublishRouteCreatedAsync(Route route);

    /// <summary>
    /// Publishes an event when an existing route is updated (ETA, reroute, etc.).
    /// </summary>
    Task PublishRouteUpdatedAsync(Guid routeId, RouteStatusEnum status, DateTime? estimatedCompletionTime);

    /// <summary>
    /// Publishes an event when a route is completed (order fulfilled, technician done).
    /// </summary>
    Task PublishRouteCompletedAsync(Route route);

    /// <summary>
    /// Publishes an event when a route is delayed due to traffic or other factors.
    /// </summary>
    Task PublishRouteDelayedAsync(Guid routeId, TimeSpan delayDuration, string reason);
    
    /// <summary>
    /// Publishes a message requesting company orders from the Order Service.
    /// </summary>
    Task PublishFetchCompanyOrdersAsync(Guid companyId);

}
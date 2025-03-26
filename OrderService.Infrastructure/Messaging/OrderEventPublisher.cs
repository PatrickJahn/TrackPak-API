using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using Shared.Messaging;
using Shared.Messaging.Events.Order;
using Shared.Messaging.Topics;
using Shared.Models;

namespace OrderService.Infrastructure.Messaging;

public class OrderEventPublisher(IMessageBus messageBus) : IOrderEventPublisher
{
    public async Task PublishOrderCreatedAsync(Order order, CreateLocationRequestModel location, CreateUserRequestModel? user)
    {
        
        var userCreatedEvent = new OrderCreatedEvent()
        {
            OrderId = order.Id,
            User = user,
            Location = location
        };

        await messageBus.PublishAsync(MessageTopic.OrderCreated, userCreatedEvent);
    }
    public async Task PublishCompanyOrdersAsync(Guid companyId, IEnumerable<Order> orders)
    {
        var companyOrdersEvent = new CompanyOrdersFetchedEvent
        {
            CompanyId = companyId,
            OrderIds = orders.Select(o => o.Id).ToList()
        };

        await messageBus.PublishAsync(MessageTopic.CompanyOrdersFetched, companyOrdersEvent);
    }

}
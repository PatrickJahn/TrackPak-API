using LocationService.Application.Interfaces;
using Shared.Messaging;
using Shared.Messaging.Events.Location;
using Shared.Messaging.Topics;

namespace LocationService.Application.Messaging;

public class LocationEventPublisher(IMessageBus messageBus) : ILocationEventPublisher
{
    public async Task PublishUserLocationCreatedAsync(UserLocationCreatedEvent message)
    {
        await messageBus.PublishAsync(MessageTopic.UserLocationCreated, message);
    }

    public async Task PublishOrderLocationCreatedAsync(OrderLocationCreatedEvent message)
    {
        await messageBus.PublishAsync(MessageTopic.OrderLocationCreated, message);
    }

    public async Task PublishCompanyLocationCreatedAsync(CompanyLocationCreatedEvent message)
    {
        await messageBus.PublishAsync(MessageTopic.CompanyLocationCreated, message);
    }

    public async Task PublishEmployeeLocationCreatedAsync(EmployeeLocationCreatedEvent message)
    {
        await messageBus.PublishAsync(MessageTopic.EmployeeLocationCreated, message);
    }
}
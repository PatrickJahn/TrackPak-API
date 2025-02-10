using Shared.Messaging.Events.Location;

namespace LocationService.Application.Interfaces;

public interface ILocationEventPublisher
{
    Task PublishUserLocationCreatedAsync(UserLocationCreatedEvent message);
    Task PublishOrderLocationCreatedAsync(OrderLocationCreatedEvent message);
    Task PublishCompanyLocationCreatedAsync(CompanyLocationCreatedEvent message);
    Task PublishEmployeeLocationCreatedAsync(EmployeeLocationCreatedEvent message);

}
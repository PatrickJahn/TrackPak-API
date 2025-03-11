using EmployeeService.Application.Interfaces;
using Shared.Messaging;
using Shared.Messaging.Events.Employee;
using Shared.Messaging.Topics;
using Shared.Models;
using Employee = EmployeeService.Domain.Entities.Employee;

namespace EmployeeService.Infrastructure.Messaging;

public class EmployeeEventPublisher(IMessageBus messageBus) : IEmployeeEventPublisher
{
    public async Task PublishEmployeeCreatedEventAsync(Employee emp, CreateLocationRequestModel location)
    {
        var empEvent = new EmployeeCreatedEvent()
        {
            EmployeeId = emp.Id,
            Location = location
        };
        
        await messageBus.PublishAsync(MessageTopic.EmployeeCreated, empEvent);
    }
   
    public async Task PublishEmployeeCheckedInEventAsync(Guid employeeId)
    {
        var empEvent = new EmployeeCheckedInEvent()
        {
            EmployeeId = employeeId,
            CheckedInAt = DateTime.UtcNow
        };
        
        await messageBus.PublishAsync(MessageTopic.EmployeeCheckedIn, empEvent);
        
    }

    public async Task PublishEmployeeCheckedOutEventAsync(Guid employeeId)
    {
        var empEvent = new EmployeeCheckedOutEvent()
        {
            EmployeeId = employeeId,
            CheckedOutAt = DateTime.UtcNow
        };
        
        await messageBus.PublishAsync(MessageTopic.EmployeeCheckedOut, empEvent);
    }
}


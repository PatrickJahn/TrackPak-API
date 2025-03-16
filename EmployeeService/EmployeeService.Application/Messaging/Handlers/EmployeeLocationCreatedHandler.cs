using EmployeeService.Application.Repositories;
using Shared.Messaging;
using Shared.Messaging.Events.Location;

namespace EmployeeService.Application.Messaging.Handlers;

public class EmployeeLocationCreatedHandler(IEmployeeRepository employeeRepository)
    : IMessageHandler<EmployeeLocationCreatedEvent>
{
    public async Task HandleAsync(EmployeeLocationCreatedEvent message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Updating employee {message.EmployeeId} with location {message.LocationId}");

        
        var employee = await employeeRepository.GetOrDefaultByIdAsync(message.EmployeeId);

        if (employee == null)
        {
            // TODO: Add logic 
            return;
        }

        employee.LocationId = message.LocationId;
        await employeeRepository.Update(employee);
    }
}

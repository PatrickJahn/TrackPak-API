using Shared.Models;
using Employee = EmployeeService.Domain.Entities.Employee;

namespace EmployeeService.Application.Interfaces;

public interface IEmployeeEventPublisher
{
    
    Task PublishEmployeeCreatedEventAsync(Employee user, CreateLocationRequestModel location);
    Task PublishEmployeeCheckedInEventAsync(Guid employeeId);
    Task PublishEmployeeCheckedOutEventAsync(Guid employeeId);
    
}
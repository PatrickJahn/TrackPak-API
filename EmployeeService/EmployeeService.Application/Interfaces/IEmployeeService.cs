using Shared.Models;
using EmployeeService.Application.Models;
using EmployeeService.Domain.Entities;
using Employee = EmployeeService.Domain.Entities.Employee;

namespace EmployeeService.Application.Interfaces;

public interface IEmployeeService
{
  public Task<Employee> GetEmployeeByIdAsync(Guid employeeId);
    
  public Task<Employee> UpdateEmployeeAsync(Guid employeeId, UpdateEmployeeModel employeeModel);
    
  public Task<Employee> UpdateEmployeeLocationAsync(Guid employeeId, UpdateLocationModel locationModel);

  public Task DeleteEmployeeAsync(Guid employeeId);

  public Task CreateEmployeeAsync(CreateEmployeeModel employeeModel);
}
using Shared.Models;
using Shared.Services;
using EmployeeService.Application.Interfaces;
using EmployeeService.Application.Models;
using EmployeeService.Application.Repositories;
using EmployeeService.Domain.Entities;

namespace EmployeeService.Application.Services;

public class EmployeeService : IEmployeeService
{
  private readonly IEmployeeRepository _employeeRepo;

  public EmployeeService(IEmployeeRepository employeeRepo)
  {
    _employeeRepo = employeeRepo ?? throw new ArgumentNullException(nameof(employeeRepo));
  }

  public async Task<Employee> GetEmployeeByIdAsync(Guid employeeId)
  {
    return await _employeeRepo.GetByIdAsync(employeeId);
  }

  public async Task<Employee> UpdateEmployeeAsync(Guid employeeId, UpdateEmployeeModel employeeModel)
  {
    var employee = await _employeeRepo.GetByIdAsync(employeeId);

    employee.FirstName = employeeModel.FirstName;
    employee.Email = employeeModel.Email;
    employee.LastName = employeeModel.LastName;
    employee.PhoneNumber = employeeModel.PhoneNumber;

    await _employeeRepo.Update(employee);

    return employee;
  }

  public Task<Employee> UpdateEmployeeLocationAsync(Guid employeeId, UpdateLocationModel locationModel)
  {
    throw new NotImplementedException();
  }

  public async Task DeleteEmployeeAsync(Guid employeeId)
  {
    await _employeeRepo.DeleteByIdAsync(employeeId);
  }

  public Task CreateEmployeeAsync(CreateEmployeeModel employeeModel)
  {
    throw new NotImplementedException();
  }

  public async Task CreateEmployee(CreateEmployeeModel employeeModel)
  {
    throw new NotImplementedException();

  }
}
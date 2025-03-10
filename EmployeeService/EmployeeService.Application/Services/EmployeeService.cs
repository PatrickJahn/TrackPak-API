using Shared.Models;
using Shared.Services;
using EmployeeService.Application.Interfaces;
using EmployeeService.Application.Models;
using EmployeeService.Application.Repositories;
using EmployeeService.Domain.Entities;
using Employee = EmployeeService.Domain.Entities.Employee;

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

  public async Task CreateEmployeeAsync(CreateEmployeeModel employeeModel)
  {
    // TODO:
    // await CheckIfUserExistWithEmail(userModel.Email, cancellationToken);
    // await CheckIfUserExistWithPhone(userModel.PhoneNumber, cancellationToken);

    var emp = new Employee()
    {
      FirstName = employeeModel.FirstName,
      LastName = employeeModel.LastName,
      PhoneNumber = employeeModel.PhoneNumber,
      Email = employeeModel.Email,
      LocationId = null
    };
    
    await _employeeRepo.AddAsync(emp);
  }

 
}
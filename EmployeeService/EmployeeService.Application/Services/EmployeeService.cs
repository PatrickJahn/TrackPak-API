using System.Diagnostics;
using Shared.Models;
using Shared.Services;
using EmployeeService.Application.Interfaces;
using EmployeeService.Application.Models;
using EmployeeService.Application.Repositories;
using EmployeeService.Domain.Entities;
using Monitoring;
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
    using var activity = LoggingService.activitySource.StartActivity("EmployeeService.GetEmployeeByIdAsync", ActivityKind.Internal);
    activity?.SetTag("employee.id", employeeId);

    LoggingService.Log.AddContext().Information("Fetching employee by ID: {EmployeeId}", employeeId);

    return await _employeeRepo.GetByIdAsync(employeeId);
  }

  public async Task<Employee?> GetEmployeeByEmailAsync(string email)
  {
    using var activity = LoggingService.activitySource.StartActivity("EmployeeService.GetEmployeeByEmailAsync", ActivityKind.Internal);
    activity?.SetTag("employee.email", email);

    LoggingService.Log.AddContext().Information("Fetching employee by email: {Email}", email);
    
    return await _employeeRepo.GetAllAsync(emp => emp.Email == email).ContinueWith(emp => emp.Result.FirstOrDefault());
  }

  public async Task CheckIn(Guid employeeId)
  {
    using var activity = LoggingService.activitySource.StartActivity("EmployeeService.CheckIn", ActivityKind.Internal);
    activity?.SetTag("employee.id", employeeId);

    LoggingService.Log.AddContext().Information("Employee {EmployeeId} checking in", employeeId);

    var employee = await _employeeRepo.GetByIdAsync(employeeId);
    employee.CheckedIn = true;
    await _employeeRepo.Update(employee);
  }
  
  public async Task CheckOut(Guid employeeId)
  {
    using var activity = LoggingService.activitySource.StartActivity("EmployeeService.CheckOut", ActivityKind.Internal);
    activity?.SetTag("employee.id", employeeId);

    LoggingService.Log.AddContext().Information("Employee {EmployeeId} checking out", employeeId);

    var employee = await _employeeRepo.GetByIdAsync(employeeId);
    employee.CheckedIn = false;
    await _employeeRepo.Update(employee);

  }

  public async Task<IEnumerable<Employee>> GetEmployeeByCompanyIdAsync(Guid companyId)
  {
    using var activity = LoggingService.activitySource.StartActivity("EmployeeService.GetEmployeeByCompanyIdAsync", ActivityKind.Internal);
    activity?.SetTag("company.id", companyId);

    LoggingService.Log.AddContext().Information("Fetching employees for company: {CompanyId}", companyId);
    
    return await _employeeRepo.GetAllAsync(employee => employee.CompanyId == companyId);
  }

  public async Task<Employee> UpdateEmployeeAsync(Guid employeeId, UpdateEmployeeModel employeeModel)
  {
    using var activity = LoggingService.activitySource.StartActivity("EmployeeService.UpdateEmployeeAsync", ActivityKind.Internal);
    activity?.SetTag("employee.id", employeeId);

    LoggingService.Log.AddContext().Information("Updating employee {EmployeeId} with data: {@EmployeeModel}", employeeId, employeeModel);

    
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
    using var activity = LoggingService.activitySource.StartActivity("EmployeeService.DeleteEmployeeAsync", ActivityKind.Internal);
    activity?.SetTag("employee.id", employeeId);

    LoggingService.Log.AddContext().Information("Deleting employee {EmployeeId}", employeeId);
    await _employeeRepo.DeleteByIdAsync(employeeId);
  }

  public async Task CreateEmployeeAsync(CreateEmployeeModel employeeModel)
  {
    // TODO:
    // await CheckIfUserExistWithEmail(userModel.Email, cancellationToken);
    // await CheckIfUserExistWithPhone(userModel.PhoneNumber, cancellationToken);
    using var activity = LoggingService.activitySource.StartActivity("EmployeeService.CreateEmployeeAsync", ActivityKind.Internal);
    activity?.SetTag("company.id", employeeModel.CompanyId);

    LoggingService.Log.AddContext().Information("Creating employee: {@Employee}", employeeModel);

    var emp = new Employee()
    {
      FirstName = employeeModel.FirstName,
      LastName = employeeModel.LastName,
      PhoneNumber = employeeModel.PhoneNumber,
      Email = employeeModel.Email,
      CompanyId = employeeModel.CompanyId,
      LocationId = null,
    };
    
    await _employeeRepo.AddAsync(emp);
  }

 
}
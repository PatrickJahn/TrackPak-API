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
  private readonly ILocationServiceClient _locationServiceClient;

  public EmployeeService(IEmployeeRepository employeeRepo, ILocationServiceClient locationServiceClient)
  {
    _employeeRepo = employeeRepo ?? throw new ArgumentNullException(nameof(employeeRepo));
    _locationServiceClient = locationServiceClient ?? throw new ArgumentNullException(nameof(locationServiceClient));
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

  public async Task DeleteEmployeeAsync(Guid employeeId)
  {
    await _employeeRepo.DeleteByIdAsync(employeeId);
  }

  public async Task CreateEmployee(CreateEmployeeModel employeeModel)
  {
    var locationId = await CreateLocation(employeeModel.Location);

    await _employeeRepo.AddAsync(new Employee
    {
      PhoneNumber = employeeModel.PhoneNumber,
      Email = employeeModel.Email,
      FirstName = employeeModel.FirstName,
      LastName = employeeModel.LastName,
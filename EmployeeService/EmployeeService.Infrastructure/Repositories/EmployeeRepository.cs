using Shared.Repositories;
using EmployeeService.Application.Repositories;
using EmployeeService.Domain.Entities;
using EmployeeService.Infrastructure.DbContext;

namespace EmployeeService.Infrastructure.Repositories;

public class EmployeeRepository(EmployeeDbContext dbContext) : BaseRepository<Employee, EmployeeDbContext>(dbContext), IEmployeeRepository
{
   
}
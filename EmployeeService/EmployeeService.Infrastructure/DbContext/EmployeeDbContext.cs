using Microsoft.EntityFrameworkCore;
using EmployeeService.Domain.Entities;

namespace EmployeeService.Infrastructure.DbContext;

public class EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
{
    
  public DbSet<Employee> Employees { get; set; }
  
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
      base.OnModelCreating(modelBuilder);
  }

}
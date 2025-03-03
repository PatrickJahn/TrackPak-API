using Microsoft.EntityFrameworkCore;
using EmployeeService.Domain.Entities;

namespace EmployeeService.Infrastructure.DbContext;

public class EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : DbContext(options)
{
  public DbSet<Employee> Employees { get; set; }

  // Utility method to apply migrations
  public void ApplyMigrations()
  {
    try
    {
      base.Database.Migrate(); // Apply pending migrations
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error applying migrations: {ex.Message}");
      throw; // Re-throw the exception if needed
    }
  }
}
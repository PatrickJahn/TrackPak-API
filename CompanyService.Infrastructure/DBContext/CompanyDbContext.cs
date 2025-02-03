using CompanyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Infrastructure.DBContext;

public class CompanyDbContext : DbContext
{
  public CompanyDbContext(DbContextOptions<CompanyDbContext> options) : base(options)
  {
  }

  public DbSet<Company> Companies { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // Example of configuring an owned type, assuming Company has an Address object
    modelBuilder.Entity<Company>()
      .OwnsOne(c => c.Address);
  }
}
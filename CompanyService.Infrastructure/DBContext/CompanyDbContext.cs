using CompanyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Infrastructure.DBContext;

public class CompanyDbContext(DbContextOptions<CompanyDbContext> options) : DbContext(options)
{
  
  public DbSet<Company> Companies { get; set; }
 
}
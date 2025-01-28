using Microsoft.EntityFrameworkCore;
using UserService.Domain.entities;

namespace UserService.Infrastructure.DbContext;

public class UserDbContext(DbContextOptions<UserDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public DbSet<User> Users { get; set; }

    
    
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


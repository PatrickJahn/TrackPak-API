using Microsoft.EntityFrameworkCore;
using RouteService.Domain.Entities;

namespace RouteService.Infrastructure.DBContext;

public class RouteDbContext(DbContextOptions<RouteDbContext> options) : DbContext(options)
{
    public DbSet<Route> Routes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Route>()
            .HasKey(r => r.Id);

        modelBuilder.Entity<Route>()
            .Property(r => r.Status);

        modelBuilder.Entity<Route>()
            .Property(r => r.CreatedAt);
    }
}


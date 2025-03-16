using Microsoft.EntityFrameworkCore;
using RouteService.Domain.Entities;
using Shared.Models;

namespace RouteService.Infrastructure.DBContext;

public class RouteDbContext(DbContextOptions<RouteDbContext> options) : DbContext(options)
{
    public DbSet<Route> Routes { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Primary Key
        modelBuilder.Entity<Route>()
            .HasKey(r => r.Id);
        
        // Orders Relationship (One Route -> Many OrderRoutes)
        modelBuilder.Entity<OrderRoute>()
            .HasKey(or => new { or.RouteId, or.Sequence }); 

        modelBuilder.Entity<OrderRoute>()
            .HasOne(or => or.Route)
            .WithMany(r => r.OrderRoutes)
            .HasForeignKey(or => or.RouteId)
            .OnDelete(DeleteBehavior.Cascade); 

        modelBuilder.Entity<OrderRoute>()
            .Property(or => or.Sequence)
            .IsRequired();

        modelBuilder.Entity<OrderRoute>()
            .Property(or => or.OrderId)
            .IsRequired();


        // Status Property
        modelBuilder.Entity<Route>()
            .Property(r => r.Status)
            .HasConversion<int>(); // Stores Enum as int in DB
        
    }

}


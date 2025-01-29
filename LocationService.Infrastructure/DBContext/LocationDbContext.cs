using LocationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocationService.Infrastructure.DBContext;

public class LocationDbContext(DbContextOptions<LocationDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public DbSet<Location> Locations { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure GeoLocation as an owned type
        modelBuilder.Entity<Location>()
            .OwnsOne(l => l.GeoLocation);
    }
   
}



using LocationService.Application.Interfaces.Repositories;
using LocationService.Domain.Entities;
using LocationService.Infrastructure.DBContext;
using Shared.Interfaces;
using Shared.Repositories;

namespace LocationService.Infrastructure.Repositories;

public class LocationRepository(LocationDbContext dbContext) : BaseRepository<Location, LocationDbContext>(dbContext), ILocationRepository
{
    
    
}

    

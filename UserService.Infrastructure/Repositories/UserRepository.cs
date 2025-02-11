using Microsoft.EntityFrameworkCore;
using Shared.Repositories;
using UserService.Domain.Repositories;
using UserService.Domain.entities;
using UserService.Infrastructure.DbContext;

namespace UserService.Infrastructure.Repositories;

public class UserRepository(UserDbContext dbContext) : BaseRepository<User, UserDbContext>(dbContext), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        IQueryable<User> query = _dbContext.Set<User>();
        return await query.FirstOrDefaultAsync(e => e.Email == email, cancellationToken);
    }

    public async Task<User?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        IQueryable<User> query = _dbContext.Set<User>();
        return await query.FirstOrDefaultAsync(e => e.PhoneNumber == phone, cancellationToken);
        
    }
}
using Shared.Repositories;
using UserService.Application.Repositories;
using UserService.Domain.entities;
using UserService.Infrastructure.DbContext;

namespace UserService.Infrastructure.Repositories;

public class UserRepository(UserDbContext dbContext) : BaseRepository<User, UserDbContext>(dbContext), IUserRepository
{
   
}
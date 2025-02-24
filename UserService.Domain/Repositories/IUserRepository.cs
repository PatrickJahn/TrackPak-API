using Shared.Interfaces;
using UserService.Domain.entities;

namespace UserService.Domain.Repositories;

public interface IUserRepository: IBaseRepository<User>
{
    
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    public Task<User?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default);

}
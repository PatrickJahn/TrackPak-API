using Shared.Models;
using UserService.Application.Models;
using UserService.Domain.entities;

namespace UserService.Application.Interfaces;

public interface IUserService
{
    public Task<User> GetUserByIdAsync(Guid userId,  CancellationToken cancellationToken);
    
    public Task<User> UpdateUserAsync(Guid userId, UpdateUserModel userModel,  CancellationToken cancellationToken);
    
    public Task UpdateUserLocationAsync(Guid userId, UpdateLocationModel locationModel,  CancellationToken cancellationToken);

    public Task DeleteUserAsync(Guid userId,  CancellationToken cancellationToken);

    public Task CreateUser(CreateUserModel userModel, CancellationToken cancellationToken);

}
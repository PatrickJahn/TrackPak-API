using Shared.Models;
using UserService.Application.Models;
using UserService.Domain.entities;

namespace UserService.Application.Interfaces;

public interface IUserService
{
    public Task<User> GetUserByIdAsync(Guid userId);
    
    public Task<User> UpdateUserAsync(Guid userId, UpdateUserModel userModel);
    
    public Task<User> UpdateUserLocationAsync(Guid userId, UpdateLocationModel locationModel);

    public Task DeleteUserAsync(Guid userId);

    public Task CreateUser(CreateUserModel userModel);

}
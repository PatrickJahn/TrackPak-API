using Shared.Models;
using UserService.Application.Interfaces;
using UserService.Application.Models;
using UserService.Application.Repositories;
using UserService.Domain.entities;

namespace UserService.Application.Services;

public class UserService: IUserService
{
    private readonly IUserRepository _userRepo;

    public UserService(IUserRepository userRepo)
    {
        _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
    }

    public async Task<User> GetUserByIdAsync(Guid userId)
    {
       return await _userRepo.GetByIdAsync(userId);
    }

    public async Task<User> UpdateUserAsync(Guid userId,UpdateUserModel userModel)
    {
        var user = await _userRepo.GetByIdAsync(userId);

        user.FirstName = userModel.FirstName;
        user.Email = userModel.Email;
        user.LastName = userModel.LastName;
        user.PhoneNumber = userModel.PhoneNumber;

        await _userRepo.Update(user);

        return user;
    }

    public Task<User> UpdateUserLocationAsync(Guid userId, UpdateLocationModel locationModel)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteUserAsync(Guid userId)
    {
       await _userRepo.DeleteByIdAsync(userId);
    }

    public async Task CreateUser(CreateUserModel userModel)
    {
        
        
        // TODO: Call Location Service to add new Location / Get Location Id - WITH A FIRE AND HOPE 
        // IF IT FAILS PUBLISH AND EVENT FOR UserCreatedWithoutLocation
        // UserService Should subscribe to Event Send from locationService, to set Location Id
        
         await _userRepo.AddAsync(new User
         {
             PhoneNumber = userModel.PhoneNumber,
             Email = userModel.Email,
             CreatedAt = DateTime.UtcNow,
             FirstName =  userModel.FirstName,
             LastName = userModel.FirstName,
             LocationId = null
         });
    }
    
    
}
using UserService.Application.Models;
using UserService.Application.Repositories;
using UserService.Domain.entities;

namespace UserService.Application.Services;

public class UserService
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
    
    public async Task CreateUser(CreateUserModel userModel)
    {
        
        
        // TODO: Call Location Service to add new Location / Get Location Id
        
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
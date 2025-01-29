using Shared.Models;
using Shared.Services;
using UserService.Application.Interfaces;
using UserService.Application.Models;
using UserService.Application.Repositories;
using UserService.Domain.entities;

namespace UserService.Application.Services;

public class UserService: IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly ILocationServiceClient _locationServiceClient;

    public UserService(IUserRepository userRepo, ILocationServiceClient locationServiceClient)
    {
        _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
        _locationServiceClient = locationServiceClient ?? throw new ArgumentNullException(nameof(locationServiceClient));
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
        
        var locationId = await CreateLocation(userModel.Location);
        
         await _userRepo.AddAsync(new User
         {
             PhoneNumber = userModel.PhoneNumber,
             Email = userModel.Email,
             FirstName =  userModel.FirstName,
             LastName = userModel.LastName,
             LocationId = locationId
         });
    }


    private async Task<Guid?> CreateLocation(CreateLocationRequestModel locationModel)
    {
        
        // TODO: Implement circuit breaker pattern. When open - send Message event instead of calling api. 
        try
        {
           var locationId = await _locationServiceClient.CreateLocationAsync(locationModel);
           return locationId;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
    
    
}
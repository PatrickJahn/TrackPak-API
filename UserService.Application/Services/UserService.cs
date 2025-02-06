using System.Net.Sockets;
using Shared.Messaging;
using Shared.Messaging.Events.User;
using Shared.Messaging.Topics;
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
    private readonly IMessageBus _messageBus;

    public UserService(IUserRepository userRepo, ILocationServiceClient locationServiceClient, IMessageBus messageBus)
    {
        _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
        _locationServiceClient = locationServiceClient ?? throw new ArgumentNullException(nameof(locationServiceClient));
        _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
    }

    public async Task<User> GetUserByIdAsync(Guid userId)
    {
       return await _userRepo.GetByIdAsync(userId);
    }

    public async Task<User> UpdateUserAsync(Guid userId,UpdateUserModel userModel)
    {
        
        // TODO:  Check if user with email or phone exists
        
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
        // TODO: Implement
        throw new NotImplementedException();
    }

    public async Task DeleteUserAsync(Guid userId)
    {
       await _userRepo.DeleteByIdAsync(userId);
    }

    public async Task CreateUser(CreateUserModel userModel)
    {
        
        
        // TODO: Check if user with same email or phone exists
        
         await _userRepo.AddAsync(new User
         {
             PhoneNumber = userModel.PhoneNumber,
             Email = userModel.Email,
             FirstName =  userModel.FirstName,
             LastName = userModel.LastName,
             LocationId = null
         });
         
         PublishUserCreatedEvent(userModel);
    }


    private void PublishUserCreatedEvent(CreateUserModel model)
    {
       
            _messageBus.PublishAsync(MessageTopic.UserCreated,
                new UserCreatedEvent()
                {
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Location = model.Location
                });

    }
}
    
    

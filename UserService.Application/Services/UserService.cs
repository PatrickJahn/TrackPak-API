using System.Net.Sockets;
using Shared.Messaging;
using Shared.Messaging.Messages;
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
        try
        {
           var locationId = await _locationServiceClient.CreateLocationAsync(locationModel);
           return locationId;
        }
        catch (Exception)
        {
            Console.WriteLine("LocationService is unreachable. Sending failure message to RabbitMQ...");
        
            _ = _messageBus.PublishAsync(MessageTopic.UserLocationCreationFailed,
                new UserLocationCreationFailedMessage()
                {
                    City = locationModel.City,
                    Country = locationModel.Country,
                    PostalCode = locationModel.PostalCode,
                    AddressLine = locationModel.AddressLine
                });

            return null;
        }
    }
    
    
}
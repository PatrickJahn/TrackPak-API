using System.Diagnostics;
using System.Net.Sockets;
using Monitoring;
using Shared.Exceptions;
using Shared.Messaging;
using Shared.Messaging.Events.User;
using Shared.Messaging.Topics;
using Shared.Models;
using Shared.Services;
using UserService.Application.Interfaces;
using UserService.Application.Models;
using UserService.Domain.Repositories;
using UserService.Domain.entities;

namespace UserService.Application.Services;

public class UserService: IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IUserEventPublisher _userEventPublisher;

    public UserService(IUserRepository userRepo, IUserEventPublisher userEventPublisher)
    {
        _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
        _userEventPublisher = userEventPublisher ?? throw new ArgumentNullException(nameof(userEventPublisher));
    }

    public async Task<User> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        using var activity = LoggingService.activitySource.StartActivity("UserService.GetUserById", ActivityKind.Internal);
        activity?.SetTag("user.id", userId);
        
       return await _userRepo.GetByIdAsync(userId);
    }

    public async Task<User> UpdateUserAsync(Guid userId, UpdateUserModel userModel, CancellationToken cancellationToken)
    {
        
        using var activity = LoggingService.activitySource.StartActivity("UserService.UpdateUser", ActivityKind.Internal);
        activity?.SetTag("user.id", userId);
        activity?.SetTag("user.email", userModel.Email);
        activity?.SetTag("user.phone", userModel.PhoneNumber);
        
        // TODO: Improve 
        await CheckIfUserExistWithEmail(userModel.Email, cancellationToken);
        await CheckIfUserExistWithPhone(userModel.PhoneNumber, cancellationToken);
        
        var user = await _userRepo.GetByIdAsync(userId);

        user.FirstName = userModel.FirstName;
        user.Email = userModel.Email;
        user.LastName = userModel.LastName;
        user.PhoneNumber = userModel.PhoneNumber;
        
        await _userRepo.Update(user);

        return user;
    }

    public async Task UpdateUserLocationAsync(Guid userId, CreateLocationRequestModel locationModel, CancellationToken cancellationToken)
    {
        
        using var activity = LoggingService.activitySource.StartActivity("UserService.UpdateUserLocation", ActivityKind.Internal);
        activity?.SetTag("user.id", userId);
        activity?.SetTag("location.city", locationModel.City);
        activity?.SetTag("location.country", locationModel.Country);

        var userExists = await _userRepo.ExistsAsync(userId);
        if (!userExists)
        {
            throw new NotFoundException("User does not exist");
        }
        
        await _userEventPublisher.PublishUserLocationUpdatedAsync(userId, locationModel);
        
    }

    public async Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
    {
       await _userRepo.DeleteByIdAsync(userId);
    }

    public async Task<Guid> CreateUser(CreateUserModel userModel, CancellationToken cancellationToken)
    {
        
        using var activity = LoggingService.activitySource.StartActivity("UserService.CreateUser", ActivityKind.Internal);
        activity?.SetTag("user.email", userModel.Email);
        activity?.SetTag("user.phone", userModel.PhoneNumber);

        
        // TODO: Improve 
        await CheckIfUserExistWithEmail(userModel.Email, cancellationToken);

        var user = new User
        {
            PhoneNumber = userModel.PhoneNumber,
            Email = userModel.Email,
            FirstName = userModel.FirstName,
            LastName = userModel.LastName,
            LocationId = null
        };
        
        await _userRepo.AddAsync(user);
         
        await _userEventPublisher.PublishUserCreatedAsync(user, userModel.Location);
        
        return user.Id;
    }

    private async Task CheckIfUserExistWithEmail(string email, CancellationToken cancellationToken)
    {
      var emailInUse = await _userRepo.GetByEmailAsync(email, cancellationToken);
      
      if(emailInUse is not null)
          throw new ConflictException($"Email {email} already exists");
    }
    
    private async Task CheckIfUserExistWithPhone( string phoneNumber, CancellationToken cancellationToken)
    {
        var phoneInUse = await _userRepo.GetByPhoneAsync(phoneNumber, cancellationToken);
      
        if(phoneInUse is not null)
            throw new ConflictException($"Phonenumber {phoneNumber} already exists");
    }


  
}
    
    

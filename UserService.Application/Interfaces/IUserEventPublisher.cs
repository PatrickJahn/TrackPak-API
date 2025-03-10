using Shared.Models;
using UserService.Domain.entities;

namespace UserService.Application.Interfaces;

public interface IUserEventPublisher
{
    Task PublishUserCreatedAsync(User user, CreateLocationRequestModel location);
    Task PublishUserLocationUpdatedAsync(Guid userId, CreateLocationRequestModel user);

}
using UserService.Application.Models;

namespace UserService.Application.Interfaces;

public interface IUserEventPublisher
{
    Task PublishUserCreatedAsync(CreateUserModel user);
}
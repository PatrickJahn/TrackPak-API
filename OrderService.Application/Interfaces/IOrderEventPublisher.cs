using OrderService.Domain.Entities;
using Shared.Models;

namespace OrderService.Application.Interfaces;

public interface IOrderEventPublisher
{
    Task PublishOrderCreatedAsync(Order order, CreateLocationRequestModel location, CreateUserRequestModel user);
}
using CompanyService.Domain.Entities;
using Shared.Messaging.Events.Company;
using Shared.Models;

namespace CompanyService.Application.Interfaces;

public interface ICompanyEventPublisher
{
    Task PublishCompanyCreatedAsync(Company company, CreateLocationRequestModel createLocationRequestModel);

}
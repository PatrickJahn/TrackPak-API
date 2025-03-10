using Shared.Models;
using Company = CompanyService.Domain.Entities.Company;

namespace CompanyService.Application.Interfaces;

public interface ICompanyEventPublisher
{
    Task PublishCompanyCreatedAsync(Company company, CreateLocationRequestModel createLocationRequestModel);

}
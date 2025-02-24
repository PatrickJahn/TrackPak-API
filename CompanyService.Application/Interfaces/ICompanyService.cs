using CompanyService.Application.Models;
using CompanyService.Domain.Entities;

namespace CompanyService.Application.Interfaces
{
  public interface ICompanyService
  {
    Task<Company> CreateCompanyAsync(CreateCompanyModel company, CancellationToken cancellationToken = default);
    Task<Company?> GetCompanyByIdAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Company>> GetCompaniesAsync(CancellationToken cancellationToken = default);
    Task<bool> UpdateCompanyAsync(Guid companyId, UpdateCompanyModel updatedCompany, CancellationToken cancellationToken = default);
    Task<bool> DeleteCompanyAsync(Guid companyId, CancellationToken cancellationToken = default);
  }
}

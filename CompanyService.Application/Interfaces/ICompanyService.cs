using CompanyService.Domain.Entities;

namespace CompanyService.Application.Interfaces
{
  public interface ICompanyService
  {
    Task<Company> CreateCompanyAsync(Company company, CancellationToken cancellationToken = default);
    Task<Company?> GetCompanyByIdAsync(string companyId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Company>> GetCompaniesAsync(CancellationToken cancellationToken = default);
    Task<bool> UpdateCompanyAsync(string companyId, Company updatedCompany, CancellationToken cancellationToken = default);
    Task<bool> DeleteCompanyAsync(string companyId, CancellationToken cancellationToken = default);
  }
}

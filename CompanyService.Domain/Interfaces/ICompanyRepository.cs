using CompanyService.Domain.Entities;
using Shared.Interfaces;
using Company = Shared.Models.Company;

namespace CompanyService.Domain.Interfaces;

public interface ICompanyRepository: IBaseRepository<Company>
{
    
}
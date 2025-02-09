using CompanyService.Application.Interfaces;
using CompanyService.Application.Models;
using CompanyService.Domain.Entities;
using CompanyService.Domain.Interfaces;

namespace CompanyService.Application.Services;

public class CompanyService(ICompanyRepository companyRepository) : ICompanyService
{
    public async Task<Company> CreateCompanyAsync(CreateCompanyModel company, CancellationToken cancellationToken = default)
    {
        var newCompany = new Company()
        {
            Name = company.Name,
            Cvr = company.Cvr,
            BrandId = company.BrandId,
            LocationId = null,
            CreatedAt = DateTime.UtcNow
        };
        
        await companyRepository.AddAsync(newCompany);
    
        // SEND event

        return newCompany;
    }
    

    public async Task<Company?> GetCompanyByIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
       return await companyRepository.GetOrDefaultByIdAsync(companyId);
    }

    public async Task<IEnumerable<Company>> GetCompaniesAsync(CancellationToken cancellationToken = default)
    {
       return await companyRepository.GetAllAsync(cancellationToken);
    }

    public async Task<bool> UpdateCompanyAsync(Guid companyId, UpdateCompanyModel updatedCompany,
        CancellationToken cancellationToken = default)
    {
        var company = await companyRepository.GetByIdAsync(companyId);
        
        company.Name = updatedCompany.Name;
        company.Cvr = updatedCompany.Cvr;
        company.BrandId = updatedCompany.BrandId;

        await companyRepository.Update(company);
        
        return true;
    }

    public async Task<bool> DeleteCompanyAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        await companyRepository.DeleteByIdAsync(companyId);
        return true;
    }
    
    
}
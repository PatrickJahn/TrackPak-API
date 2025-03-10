using CompanyService.Application.Interfaces;
using CompanyService.Application.Models;
using CompanyService.Domain.Entities;
using CompanyService.Domain.Interfaces;

namespace CompanyService.Application.Services;

public class CompanyService(ICompanyRepository companyRepository, ICompanyEventPublisher eventPublisher) : ICompanyService
{
    public async Task<Company> CreateCompanyAsync(CreateCompanyModel model, CancellationToken cancellationToken = default)
    {
        
        // TODO: Check if company with same cvr exists and handle
        
        var newCompany = new Company()
        {
            Name = model.Name,
            Cvr = model.Cvr,
            BrandId = model.BrandId,
            LocationId = null,
            CreatedAt = DateTime.UtcNow
        };
        
        await companyRepository.AddAsync(newCompany);
        
        await eventPublisher.PublishCompanyCreatedAsync(newCompany, model.Location);

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
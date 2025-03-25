using System.Diagnostics;
using CompanyService.Application.Interfaces;
using CompanyService.Application.Models;
using CompanyService.Domain.Entities;
using CompanyService.Domain.Interfaces;
using Monitoring;

namespace CompanyService.Application.Services;

public class CompanyService(ICompanyRepository companyRepository, ICompanyEventPublisher eventPublisher) : ICompanyService
{
    public async Task<Company> CreateCompanyAsync(CreateCompanyModel model, CancellationToken cancellationToken = default)
    {
        
        using var activity = LoggingService.activitySource.StartActivity("CompanyService.CreateCompanyAsync");

        activity?.SetTag("company.cvr", model.Cvr);
        activity?.SetTag("company.brandId", model.BrandId);
    
        var newCompany = new Company
        {
            Name = model.Name,
            Cvr = model.Cvr,
            BrandId = model.BrandId,
            LocationId = null,
            CreatedAt = DateTime.UtcNow
        };

        LoggingService.Log.AddContext().Information("Creating new company: {@Company}", newCompany);

        await companyRepository.AddAsync(newCompany);
        await eventPublisher.PublishCompanyCreatedAsync(newCompany, model.Location);

        return newCompany;
    }
    

    public async Task<Company?> GetCompanyByIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        using var activity = LoggingService.activitySource.StartActivity("Company servcie GetCompanyByIdAsync");

       return await companyRepository.GetOrDefaultByIdAsync(companyId);
    }

    public async Task<IEnumerable<Company>> GetCompaniesAsync(CancellationToken cancellationToken = default)
    { 
        using var activity = LoggingService.activitySource.StartActivity("Company servcie GetCompaniesAsync");
       return await companyRepository.GetAllAsync(cancellationToken);
    }

    public async Task<bool> UpdateCompanyAsync(Guid companyId, UpdateCompanyModel updatedCompany,
        CancellationToken cancellationToken = default)
    {
        using var activity = LoggingService.activitySource.StartActivity("Company servcie UpdateCompanyAsync");

        var company = await companyRepository.GetByIdAsync(companyId);
        
        company.Name = updatedCompany.Name;
        company.Cvr = updatedCompany.Cvr;
        company.BrandId = updatedCompany.BrandId;

        await companyRepository.Update(company);
        
        return true;
    }

    public async Task<bool> DeleteCompanyAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        using var activity = LoggingService.activitySource.StartActivity("Company servcie DeleteCompanyAsync");

        await companyRepository.DeleteByIdAsync(companyId);
        return true;
    }
    
    
}
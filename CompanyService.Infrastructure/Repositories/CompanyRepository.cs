using CompanyService.Domain.Entities;
using CompanyService.Domain.Interfaces;
using CompanyService.Infrastructure.DBContext;
using Shared.Repositories;

namespace CompanyService.Infrastructure.Repositories;

public class CompanyRepository(CompanyDbContext dbContext) : BaseRepository<Company, CompanyDbContext>(dbContext), ICompanyRepository
{
    
}
using System.Linq.Expressions;
using CompanyService.Domain.Entities;
using CompanyService.Domain.Interfaces;
using CompanyService.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore.Query;
using Shared.Repositories;
using Company = Shared.Models.Company;

namespace CompanyService.Infrastructure.Repositories;

public class CompanyRepository(CompanyDbContext dbContext) : BaseRepository<Company, CompanyDbContext>(dbContext), ICompanyRepository
{
    public Task<Domain.Entities.Company> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Domain.Entities.Company?> GetOrDefaultByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Domain.Entities.Company>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Domain.Entities.Company>> GetAllAsync(Expression<Func<Domain.Entities.Company, bool>> predicate, CancellationToken cancellationToken = default, Func<IQueryable<Domain.Entities.Company>, IIncludableQueryable<Domain.Entities.Company, object?>>? include = null)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Domain.Entities.Company>> GetAllAsync(List<Guid> ids, CancellationToken cancellationToken = default, Func<IQueryable<Domain.Entities.Company>, IIncludableQueryable<Domain.Entities.Company, object?>>? include = null)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Domain.Entities.Company entity)
    {
        throw new NotImplementedException();
    }

    public Task Update(Domain.Entities.Company entity)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Domain.Entities.Company entity)
    {
        throw new NotImplementedException();
    }

    public Task ForceDelete(Domain.Entities.Company entity)
    {
        throw new NotImplementedException();
    }

    public Task ForceDeleteRange(List<Domain.Entities.Company> entities)
    {
        throw new NotImplementedException();
    }
}
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Shared.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : class
{
    Task<TEntity> GetByIdAsync(Guid id);
    Task<TEntity?> GetOrDefaultByIdAsync(Guid id);

    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    
    Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null);
    
    Task<IEnumerable<TEntity>> GetAllAsync(List<Guid> ids, CancellationToken cancellationToken = default,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null);
    
    Task AddAsync(TEntity entity);
    Task Update(TEntity entity);
    Task DeleteByIdAsync(Guid id);
    Task Delete(TEntity entity);
    Task ForceDelete(TEntity entity);
    Task ForceDeleteRange(List<TEntity> entities);
    Task<bool> ExistsAsync(Guid id);
}
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Shared.Exceptions;
using Shared.Models;
using Shared.Interfaces;

namespace Shared.Repositories;

public class BaseRepository<TEntity, TContext> : IBaseRepository<TEntity> where TEntity : BaseModel where TContext : DbContext
{
    protected readonly TContext _dbContext;
    
    protected BaseRepository(TContext dbContext)
    {
        _dbContext = dbContext;

    }

    public async Task<TEntity> GetByIdAsync(Guid id){
        
        var ret = await GetOrDefaultByIdAsync(id);
        if (ret == null)
          throw new NotFoundException($"Cannot find {typeof(TEntity).Name} with id {id}");
                
        return ret;
    }

    public async Task<TEntity?> GetOrDefaultByIdAsync(Guid id)
    {
        // Start building the query
        IQueryable<TEntity> query = _dbContext.Set<TEntity>();
        
        // Query by ID and return the result
        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }



    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default){
        var query = _dbContext.Set<TEntity>();
        return await query.ToListAsync(cancellationToken);
    }


    public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate,
      CancellationToken cancellationToken = default,
      Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null)
    {
      var query = _dbContext.Set<TEntity>()
        .Where(predicate);

      // Apply eager loading for each navigation property provided
      if (include != null)
      {
        query = include(query); // Apply the includes
      }
      return await query.ToListAsync(cancellationToken);

    }

    
    public async Task<IEnumerable<TEntity>> GetAllAsync(List<Guid> ids,
      CancellationToken cancellationToken = default,
      Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null) {
      // Build the predicate for filtering based on the IDs list
      Expression<Func<TEntity, bool>> idPredicate = entity => ids.Contains(entity.Id);

      // Apply the DefaultPredicate and the idPredicate
      var query = _dbContext.Set<TEntity>()
        .Where(idPredicate);     // Apply the filter for matching the list of GUIDs

      // Apply eager loading for each navigation property provided
      if (include != null)
      {
        query = include(query); // Apply the includes
      }

      return await query.ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate,
      CancellationToken cancellationToken = default) {
      var query = _dbContext.Set<TEntity>()
        .Where(predicate);
    
      return await query.CountAsync(cancellationToken);
    }

    public async Task AddAsync(TEntity entity) {
      entity.CreatedAt = DateTime.UtcNow;
      entity.LastModifiedAt = DateTime.UtcNow;
      await _dbContext.Set<TEntity>().AddAsync(entity);
      await _dbContext.SaveChangesAsync();
    }

    public async Task Update(TEntity entity) {
      entity.LastModifiedAt = DateTime.UtcNow;
      _dbContext.Set<TEntity>().Update(entity);
      await _dbContext.SaveChangesAsync();
    }

   

    public async Task DeleteByIdAsync(Guid id) {
      var entity = await _dbContext.Set<TEntity>().FindAsync(id);
      if (entity != null) {
        await Delete(entity);
      }
    }

    public async Task Delete(TEntity entity) {
      if (entity is ISoftDeleteEntity softDeletableEntity)
      {
        // Mark the entity as deleted
        softDeletableEntity.Deleted = DateTime.UtcNow;
        softDeletableEntity.DeletedBy = "user";
        _dbContext.Set<TEntity>().Update(entity);
        await _dbContext.SaveChangesAsync();
        return;
      }

      // Physically remove the entity from the database
      _dbContext.Set<TEntity>().Remove(entity);
      await _dbContext.SaveChangesAsync();
    }

    public async Task ForceDelete(TEntity entity) {
      _dbContext.Remove(entity);
      await _dbContext.SaveChangesAsync();
    }

    public async Task ForceDeleteRange(List<TEntity> entities) {
      _dbContext.RemoveRange(entities);
      await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
      var query = _dbContext.Set<TEntity>();
     
      return await query.AnyAsync(e => ( e.Id == id));
    }

}
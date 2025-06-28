using EntityFramework.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;

namespace EntityFramework.Repositories;

public class Repository<T>(
    HestiaContext context,
    ILogger<Repository<T>> logger
    ) : IRepository<T> where T : class
{

    public IQueryable<T> Query(bool asNoTracking = true)
    {
        logger.LogInformation($"Querying entities of type {typeof(T).Name}");
        var query = context.Set<T>().AsQueryable();
        return asNoTracking ? query.AsNoTracking() : query;
    }

    public async Task<List<T>> GetAll(Guid colocationId)
    {
        logger.LogInformation($"Getting all entities of type {typeof(T).Name} for colocation {colocationId}");
        return await context.Set<T>()
                            .AsNoTracking()
                            .ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        logger.LogInformation($"Getting entity of type {typeof(T).Name} with id {id}");
        return await context.Set<T>().FindAsync(id);
    }

    public async Task<T> AddAsync(T entity)
    {
        try
        {
            await context.Set<T>().AddAsync(entity);
            logger.LogInformation($"Adding entity of type {typeof(T).Name}");
            return entity;
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Error adding entity of type {typeof(T).Name}");
            throw new ContextException($"Could not Add {typeof(T).Name} changes. Check inner exception for details.");
        }
    }

    public async Task<int> AddRangeAsync(IEnumerable<T> entities)
    {
        try
        {
            await context.Set<T>().AddRangeAsync(entities);
            logger.LogInformation($"Adding entities of type {typeof(T).Name}");
            return entities.Count();
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Error adding entities of type {typeof(T).Name}");
            throw new ContextException($"Could not Add range {typeof(T).Name} changes. Check inner exception for details.");

        }
    }

    public T Update(T entity)
    {
        try
        {
            context.Set<T>().Update(entity);
            logger.LogInformation($"Updating entity of type {typeof(T).Name}");
            return entity;
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Error updating entity of type {typeof(T).Name}");
            throw new ContextException($"Could not Update {typeof(T).Name} changes. Check inner exception for details.");
        }
    }

    public int UpdateRange(IEnumerable<T> entities)
    {
        try
        {
            context.Set<T>().UpdateRange(entities);
            logger.LogInformation($"Updating entities of type {typeof(T).Name}");
            return entities.Count();
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Error updating entities of type {typeof(T).Name}");
            throw new ContextException($"Could not Update range {typeof(T).Name} changes. Check inner exception for details.");
        }
    }

    public T Delete(T entity)
    {
        try
        {
            logger.LogInformation($"Deleting entity of type {typeof(T).Name}");
            context.Set<T>().Remove(entity);
            return entity;
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Error deleting entity of type {typeof(T).Name}");
            throw new ContextException($"Could not Delete {typeof(T).Name} changes. Check inner exception for details.");
        }
    }

    public async Task<T> DeleteFromIdAsync(Guid id)
    {
        try
        {
            logger.LogInformation($"Deleting entity of type {typeof(T).Name} with id {id}");
            var entity = await context.Set<T>().FindAsync(id);
            if (entity == null)
            {
                logger.LogWarning($"Entity of type {typeof(T).Name} with id {id} not found");
                throw new NotFoundException($"Entity of type {typeof(T).Name} with id {id} not found");
            }
            context.Set<T>().Remove(entity);
            return entity;
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Error deleting entity of type {typeof(T).Name} with id {id}");
            throw new ContextException($"Could not Delete by Id {typeof(T).Name} changes. Check inner exception for details.");
        }
    }

    public int DeleteRangeAsync(IEnumerable<T> entities)
    {
        try
        {
            logger.LogInformation($"Deleting entities of type {typeof(T).Name}");
            context.Set<T>().RemoveRange(entities);
            return entities.Count();
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Error deleting entities of type {typeof(T).Name}");
            throw new ContextException($"Could not Delete range {typeof(T).Name} changes. Check inner exception for details.");
        }
    }

    public async Task SaveChangesAsync()
    {
        try
        {
            logger.LogInformation("Saving changes...");
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException dbEx)
        {
            logger.LogError(dbEx, "Database update failed during SaveChanges");

            throw new ContextException(
                "A database error occurred while saving changes. Check the inner exception and logs for more details.",
                dbEx
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during SaveChanges");

            throw new ContextException(
                "An unexpected error occurred while saving changes. Please contact support or check logs.",
                ex
            );
        }
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        logger.LogInformation("Beginning transaction");
        return await context.Database.BeginTransactionAsync();
    }
}

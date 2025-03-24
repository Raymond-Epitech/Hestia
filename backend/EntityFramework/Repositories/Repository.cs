using EntityFramework.Context;
using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;
using System.Linq.Expressions;

namespace EntityFramework.Repositories;

public class Repository<T>(
    HestiaContext context,
    ILogger<Repository<T>> logger
    ) : IRepository<T> where T : class, IEntity
{
    public async Task<List<T>> GetAll(Guid colocationId)
    {
        logger.LogInformation($"Getting all entities of type {typeof(T).Name} for colocation {colocationId}");
        return await context.Set<T>()
                            .AsNoTracking()
                            .ToListAsync();
    }

    public async Task<List<T>> GetAllByList(List<Guid> idList)
    {
        logger.LogInformation($"Getting all entities of type {typeof(T).Name} from list");
        return await context.Set<T>()
                            .AsNoTracking()
                            .Where(x => idList.Contains(x.Id))
                            .ToListAsync();
    }

    public async Task<List<TResult>> GetAllAsTypeAsync<TResult>(Expression<Func<T, TResult>> selector)
    {
        return await context.Set<T>()
                            .AsNoTracking()
                            .Select(selector)
                            .ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        logger.LogInformation($"Getting entity of type {typeof(T).Name} with id {id}");
        return await context.Set<T>().FindAsync(id);
    }

    public async Task<TResult?> GetByIdAsTypeAsync<TResult>(Guid id, Expression<Func<T, TResult>> selector)
    {
        return await context.Set<T>()
                            .AsNoTracking()
                            .Where(x => x.Id == id)
                            .Select(selector)
                            .FirstOrDefaultAsync();
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
            throw new ContextException("Error during the changing of information, see log for better details");
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
            throw new ContextException("Error during the changing of information, see log for better details");
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
            throw new ContextException("Error during the changing of information, see log for better details");
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
            throw new ContextException("Error during the changing of information, see log for better details");
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
            throw new ContextException("Error during the changing of information, see log for better details");
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
            throw new ContextException("Error during the changing of information, see log for better details");
        }
    }

    public async Task SaveChangesAsync()
    {
        try
        {
            logger.LogInformation("Saving changes");
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error saving changes");
            throw new ContextException("Error during the changing of information, see log for better details");
        }
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        logger.LogInformation("Beginning transaction");
        return await context.Database.BeginTransactionAsync();
    }
}

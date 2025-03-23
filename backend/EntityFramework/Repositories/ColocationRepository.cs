using EntityFramework.Context;
using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace EntityFramework.Repositories
{
    public class ColocationRepository<T>(
        HestiaContext context,
        ILogger<Repository<T>> logger
        ) : IColocationRepository<T> where T : class, IColocationEntity
    {
        public async Task<List<T>> GetAllByColocationIdAsync(Guid colocationId)
        {
            logger.LogInformation($"Getting all entities of type {typeof(T).Name} for colocation {colocationId}");
            return await context.Set<T>()
                                .AsNoTracking()
                                .Where(x => x.ColocationId == colocationId)
                                .ToListAsync();
        }

        public async Task<List<TResult>> GetAllByColocationIdAsTypeAsync<TResult>(Guid colocationId,
            Expression<Func<T, TResult>> selector)
        {
            return await context.Set<T>()
                                .AsNoTracking()
                                .Where(x => x.ColocationId == colocationId)
                                .Select(selector)
                                .ToListAsync();
        }
    }
}

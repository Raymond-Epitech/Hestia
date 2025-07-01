using Business.Interfaces;
using EntityFramework.Models;
using EntityFramework.Repositories;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Business.Services;

public class ExpiredChoreRemover(ILogger<ExpiredChoreRemover> logger,
    IRepository<Chore> repository,
    IAppCache cache) : IExpiredChoreRemover
{
    public async Task PurgeOldDataAsync()
    {
        logger.LogInformation("Purging old data from the database...");
        
        var oldChores = await repository.Query().Where(c => c.IsDone && c.DueDate < DateTime.Now).ToListAsync();
        if (!oldChores.Any())
        {
            logger.LogInformation("No old chores found to purge.");
            return;
        }

        logger.LogInformation($"Found {oldChores.Count} old chores to purge.");
        var colocationIds = oldChores.Select(c => c.ColocationId).Distinct().ToList();
        repository.DeleteRangeAsync(oldChores);
        await repository.SaveChangesAsync();

        foreach (var colocationId in colocationIds)
        {
            cache.Remove($"chores:{colocationId}");
            logger.LogInformation($"Cache cleared for colocation {colocationId}.");
        }
    }
}


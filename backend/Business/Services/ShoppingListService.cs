using Business.Interfaces;
using EntityFramework.Models;
using EntityFramework.Repositories;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Models.Output;

namespace Business.Services;

public class ShoppingListService(ILogger<ShoppingListService> logger,
    IRepository<ShoppingList> shoppingListRepository,
    IRepository<ShoppingItem> itemRepository,
    IAppCache cache) : IShoppingListService
{
    public async Task<List<ShoppingListName>> GetAllShoppingListName(Guid colocationId)
    {
        var cacheKey = $"shoppingList:{colocationId}";

        return await cache.GetOrAddAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            var shoppingListNames = await shoppingListRepository.Query()
            .Where(x => x.ColocationId == colocationId)
            .Select(x => new ShoppingListName
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();

            logger.LogInformation("Succes : All shopping list names were retrived from db");

            return shoppingListNames;
        });
    }
}


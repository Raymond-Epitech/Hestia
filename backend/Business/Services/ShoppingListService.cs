using Business.Interfaces;
using EntityFramework.Models;
using EntityFramework.Repositories;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Services;

public class ShoppingListService(ILogger<ShoppingListService> logger,
    IRepository<ShoppingList> shoppingListRepository,
    IRepository<ShoppingItem> itemRepository,
    IAppCache cache) : IShoppingListService
{
    public async Task<List<ShoppingListName>> GetAllShoppingListNameAsync(Guid colocationId)
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

    public async Task<ShoppingListOutput?> GetShoppingListAsync(Guid shoppingListId)
    {
        var shoppingList = await shoppingListRepository.Query()
            .Include(x => x.ShoppingItems)
            .Select(x => new ShoppingListOutput
            {
                Id = x.Id,
                Name = x.Name,
                ShoppingItems = x.ShoppingItems.ToList()
                    .Select(i => new ShoppingItemOutput
                    {
                        Id = i.Id,
                        Name = i.Name,
                        IsChecked = i.IsChecked
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(x => x.Id == shoppingListId);

        return shoppingList;
    }

    public async Task<Guid> AddShoppingList(ShoppingListInput shoppingListInput)
    {
        var shoppingList = new ShoppingList
        {
            Id = Guid.NewGuid(),
            CreatedBy = shoppingListInput.CreatedBy,
            Name = shoppingListInput.Name,
            ColocationId = shoppingListInput.ColocationId
        };
        var addedShoppingList = await shoppingListRepository.AddAsync(shoppingList);
        await shoppingListRepository.SaveChangesAsync();
        return addedShoppingList.Id;
    }

    public async Task<Guid> AddShoppingItem(ShoppingItemInput shoppingItemInput)
    {
        var shoppingItem = new ShoppingItem
        {
            Id = Guid.NewGuid(),
            Name = shoppingItemInput.Name,
            IsChecked = shoppingItemInput.IsChecked,
            ShoppingListId = shoppingItemInput.ShoppingListId
        };
        var addedShoppingItem = await itemRepository.AddAsync(shoppingItem);
        await itemRepository.SaveChangesAsync();
        return addedShoppingItem.Id;
    }

    public async Task<Guid> UpdateShoppingItem(ShoppingItemUpdate shoppingItemInput)
    {
        var shoppingItem = await itemRepository.GetByIdAsync(shoppingItemInput.Id);
        
        if (shoppingItem == null)
            throw new NotFoundException("Shopping item not found");

        shoppingItem.Name = shoppingItemInput.Name;
        shoppingItem.IsChecked = shoppingItemInput.IsChecked;
        itemRepository.Update(shoppingItem);
        await itemRepository.SaveChangesAsync();
        
        return true;
    }
}


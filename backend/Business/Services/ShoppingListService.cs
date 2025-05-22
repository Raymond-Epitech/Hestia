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
    /// <summary>
    /// Get all shopping list name for displaying
    /// </summary>
    /// <param name="colocationId">The colocation of the shoppinglist</param>
    /// <returns>The list with all the name of the different shopping list</returns>
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

    /// <summary>
    /// Get a shopping list with the shopping item inside
    /// </summary>
    /// <param name="shoppingListId">The id of the shopping list</param>
    /// <returns>The elemnt of the shopping list and the shopping item inside</returns>
    public async Task<ShoppingListOutput?> GetShoppingListAsync(Guid shoppingListId)
    {
        logger.LogInformation($"Shopping list: {shoppingListId}");

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

        logger.LogInformation("Succesfully retrieved all shopping lists");

        return shoppingList;
    }

    /// <summary>
    /// Add a new empty shopping list
    /// </summary>
    /// <param name="shoppingListInput">The param of the new shopping list</param>
    /// <returns>The id of the new shopping list</returns>
    public async Task<Guid> AddShoppingList(ShoppingListInput shoppingListInput)
    {
        logger.LogInformation("Add shopping list");

        var shoppingList = new ShoppingList
        {
            Id = Guid.NewGuid(),
            CreatedBy = shoppingListInput.CreatedBy,
            Name = shoppingListInput.Name,
            ColocationId = shoppingListInput.ColocationId
        };
        await shoppingListRepository.AddAsync(shoppingList);
        await shoppingListRepository.SaveChangesAsync();

        logger.LogInformation($"Succesfully add shopping list {shoppingList.Id}");

        return shoppingList.Id;
    }

    /// <summary>
    /// Add a shopping item into a shopping list
    /// </summary>
    /// <param name="shoppingItemInput">The param to add a new shopping item</param>
    /// <returns>The id of the newly created shopping item</returns>
    public async Task<Guid> AddShoppingItem(ShoppingItemInput shoppingItemInput)
    {
        logger.LogInformation("Add shopping item");

        var shoppingItem = new ShoppingItem
        {
            Id = Guid.NewGuid(),
            Name = shoppingItemInput.Name,
            IsChecked = shoppingItemInput.IsChecked,
            ShoppingListId = shoppingItemInput.ShoppingListId
        };
        await itemRepository.AddAsync(shoppingItem);
        await itemRepository.SaveChangesAsync();

        logger.LogInformation($"Succesfully added shopping item {shoppingItem.Id}");

        return shoppingItem.Id;
    }

    /// <summary>
    /// Update the name of a shopping list
    /// </summary>
    /// <param name="shoppingListUpdate">The new name of the shopping list and its id</param>
    /// <returns>The id of the updated shoppinglist</returns>
    /// <exception cref="NotFoundException">The shopping list was not found</exception>
    public async Task<Guid> UpdateShoppingList(ShoppingListUpdate shoppingListUpdate)
    {
        logger.LogInformation($"Update shopping list {shoppingListUpdate.Id}");

        var shoppingList = await shoppingListRepository.GetByIdAsync(shoppingListUpdate.Id);
        if (shoppingList == null)
            throw new NotFoundException($"Shopping list {shoppingListUpdate.Id} was not found");
        shoppingList.Name = shoppingListUpdate.Name;
        shoppingListRepository.Update(shoppingList);
        await shoppingListRepository.SaveChangesAsync();

        logger.LogInformation($"Succesfully update {shoppingListUpdate.Id}");

        return shoppingList.Id;
    }

    /// <summary>
    /// Update a shopping item
    /// </summary>
    /// <param name="shoppingItemUpdate">The new value of the shoppingitem</param>
    /// <returns>The id of the updated shopping item</returns>
    /// <exception cref="NotFoundException">The shopping item was not found</exception>
    public async Task<Guid> UpdateShoppingItem(ShoppingItemUpdate shoppingItemUpdate)
    {
        logger.LogInformation($"Update shopping item {shoppingItemUpdate.Id}");

        var shoppingItem = await itemRepository.GetByIdAsync(shoppingItemUpdate.Id);
        
        if (shoppingItem == null)
            throw new NotFoundException("Shopping item not found");

        shoppingItem.Name = shoppingItemUpdate.Name;
        shoppingItem.IsChecked = shoppingItemUpdate.IsChecked;
        itemRepository.Update(shoppingItem);
        await itemRepository.SaveChangesAsync();

        logger.LogInformation($"Succesfully updated item {shoppingItemUpdate.Id}");
        
        return shoppingItem.Id;
    }

    /// <summary>
    /// Delete a shopping list
    /// </summary>
    /// <param name="shoppingListId">The id of the shopping list</param>
    /// <returns>The if of the deleted shopping list</returns>
    public async Task<Guid> DeleteShoppingList(Guid shoppingListId)
    {
        await shoppingListRepository.DeleteFromIdAsync(shoppingListId);
        logger.LogInformation($"shopping list {shoppingListId} deleted");
        return shoppingListId;
    }

    /// <summary>
    /// Delete a item
    /// </summary>
    /// <param name="shoppingItemId">The id of the item to be deleted</param>
    /// <returns>The id of the deleted item</returns>
    public async Task<Guid> DeleteShoppingItem(Guid shoppingItemId)
    {
        await itemRepository.DeleteFromIdAsync(shoppingItemId);
        logger.LogInformation($"shopping item {shoppingItemId} deleted");
        return shoppingItemId;
    }
}


using Business.Interfaces;
using EntityFramework.Models;
using EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Update;

namespace Business.Services;

public class ShoppingListService(ILogger<ShoppingListService> logger,
    IRepository<ShoppingItem> itemRepository) : IShoppingListService
{
    public async Task<List<ShoppingItem>> GetAllShoppingItemsAsync(Guid reminderId)
    {
        logger.LogInformation("Get all shopping items");

        var items = await itemRepository.Query()
            .Where(i => i.ShoppingListReminderId == reminderId)
            .ToListAsync();

        logger.LogInformation($"Succes : All {items.Count} shopping items found");

        return items;
    }

    public async Task<Guid> AddShoppingItemAsync(ShoppingItemInput shoppingItemInput)
    {
        logger.LogInformation("Add shopping item");

        var shoppingItem = new ShoppingItem
        {
            Id = Guid.NewGuid(),
            Name = shoppingItemInput.Name,
            IsChecked = shoppingItemInput.IsChecked,
            ShoppingListReminderId = shoppingItemInput.ReminderId
        };
        await itemRepository.AddAsync(shoppingItem);
        await itemRepository.SaveChangesAsync();

        logger.LogInformation($"Succesfully added shopping item {shoppingItem.Id}");

        return shoppingItem.Id;
    }

    /// <summary>
    /// Update a shopping item
    /// </summary>
    /// <param name="shoppingItemUpdate">The new value of the shoppingitem</param>
    /// <returns>The id of the updated shopping item</returns>
    /// <exception cref="NotFoundException">The shopping item was not found</exception>
    public async Task<Guid> UpdateShoppingItemAsync(ShoppingItemUpdate shoppingItemUpdate)
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
    /// Delete a item
    /// </summary>
    /// <param name="shoppingItemId">The id of the item to be deleted</param>
    /// <returns>The id of the deleted item</returns>
    public async Task<Guid> DeleteShoppingItemAsync(Guid shoppingItemId)
    {
        await itemRepository.DeleteFromIdAsync(shoppingItemId);
        await itemRepository.SaveChangesAsync();
        logger.LogInformation($"shopping item {shoppingItemId} deleted");
        return shoppingItemId;
    }
}


using Business.Interfaces;
using Business.Mappers;
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
    IRepository<ShoppingItem> itemRepository,
    IRealTimeService realTimeService,
    IRepository<Reminder> reminderRepository,
    IAppCache cache) : IShoppingListService
{
    public async Task<List<ShoppingItemOutput>> GetAllShoppingItemsAsync(Guid reminderId)
    {
        logger.LogInformation("Get all shopping items");

        var items = await itemRepository.Query()
            .Where(i => i.ShoppingListReminderId == reminderId)
            .Select(i => i.ToOutput())
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

        var colocationId = await reminderRepository.Query().Select(r => r.ColocationId).FirstOrDefaultAsync();

        cache.Remove($"reminders:{colocationId}");

        await realTimeService.SendToGroupAsync(colocationId, "NewShoppingItem", shoppingItem.ToOutput());

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

        var shoppingItem = await itemRepository.Query()
            .Include(s => s.ShoppingListReminder)
            .FirstOrDefaultAsync(s => s.Id == shoppingItemUpdate.Id);
        
        if (shoppingItem == null)
            throw new NotFoundException("Shopping item not found");

        shoppingItem.Name = shoppingItemUpdate.Name;
        shoppingItem.IsChecked = shoppingItemUpdate.IsChecked;
        itemRepository.Update(shoppingItem);
        await itemRepository.SaveChangesAsync();

        cache.Remove($"reminders:{shoppingItem.ShoppingListReminder.ColocationId}");

        await realTimeService.SendToGroupAsync(shoppingItem.ShoppingListReminder.ColocationId, "UpdatedShoppingItem", shoppingItem.ToOutput());

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
        var toDelete = await itemRepository.Query()
            .Include(s => s.ShoppingListReminder)
            .FirstOrDefaultAsync(s => s.Id == shoppingItemId);

        if ( toDelete == null )
            throw new NotFoundException("Shopping item not found");

        itemRepository.Delete(toDelete);
        await itemRepository.SaveChangesAsync();

        logger.LogInformation($"shopping item {shoppingItemId} deleted");

        cache.Remove($"reminders:{toDelete.ShoppingListReminder.ColocationId}");

        await realTimeService.SendToGroupAsync(toDelete.ShoppingListReminder.ColocationId, "DeletedShoppingItem", shoppingItemId);

        return shoppingItemId;
    }
}


using EntityFramework.Models;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Interfaces;

public interface IShoppingListService
{
    Task<List<ShoppingItem>> GetAllShoppingItemsAsync(Guid reminderId);
    Task<Guid> AddShoppingItemAsync(ShoppingItemInput shoppingItemInput);
    Task<Guid> UpdateShoppingItemAsync(ShoppingItemUpdate shoppingItemUpdate);
    Task<Guid> DeleteShoppingItemAsync(Guid shoppingItemId);
}


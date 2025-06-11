using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Interfaces;

public interface IShoppingListService
{
    Task<List<ShoppingListName>> GetAllShoppingListNameAsync(Guid colocationId);
    Task<ShoppingListOutput?> GetShoppingListAsync(Guid shoppingListId);
    Task<Guid> AddShoppingList(ShoppingListInput shoppingListInput);
    Task<Guid> AddShoppingItem(ShoppingItemInput shoppingItemInput);
    Task<Guid> UpdateShoppingList(ShoppingListUpdate shoppingListUpdate);
    Task<Guid> UpdateShoppingItem(ShoppingItemUpdate shoppingItemUpdate);
    Task<Guid> DeleteShoppingList(Guid shoppingListId);
    Task<Guid> DeleteShoppingItem(Guid shoppingItemId);
}


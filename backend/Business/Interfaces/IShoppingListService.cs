using Shared.Models.Output;

namespace Business.Interfaces;

public interface IShoppingListService
{
    Task<List<ShoppingListName>> GetAllShoppingListName(Guid colocationId);
}


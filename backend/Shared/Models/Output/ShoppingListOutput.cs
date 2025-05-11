namespace Shared.Models.Output;

public class ShoppingListOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public List<ShoppingItemOutput> ShoppingItems { get; set; } = null!;
}


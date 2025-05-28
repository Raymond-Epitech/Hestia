namespace Shared.Models.Output;

public class ShoppingItemOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsChecked { get; set; } = false;
}


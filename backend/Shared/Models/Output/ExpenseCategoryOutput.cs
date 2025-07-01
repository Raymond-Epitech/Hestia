namespace Shared.Models.Output;

public class ExpenseCategoryOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal TotalAmount { get; set; }
}


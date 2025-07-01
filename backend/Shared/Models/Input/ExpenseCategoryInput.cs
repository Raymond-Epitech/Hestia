namespace Shared.Models.Input;
public class ExpenseCategoryInput
{
    public string Name { get; set; } = string.Empty;
    public Guid ColocationId { get; set; }
}

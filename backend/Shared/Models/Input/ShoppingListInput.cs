using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Input;

public class ShoppingListInput
{
    [Required]
    public Guid CreatedBy { get; set; }

    [Required]
    public Guid ColocationId { get; set; }

    [Required]
    public string Name { get; set; } = null!;
}


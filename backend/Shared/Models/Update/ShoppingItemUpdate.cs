using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Update;

public class ShoppingItemUpdate
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public Guid CreatedBy { get; set; }

    [Required]
    public Guid ShoppingListId { get; set; }

    [Required]
    public bool IsChecked { get; set; } = false;
}


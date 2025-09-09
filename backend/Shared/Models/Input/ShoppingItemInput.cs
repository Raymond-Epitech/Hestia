using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Input;

public class ShoppingItemInput
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public Guid CreatedBy { get; set; }

    [Required]
    public Guid ReminderId { get; set; }

    [Required]
    public bool IsChecked { get; set; } = false;
}


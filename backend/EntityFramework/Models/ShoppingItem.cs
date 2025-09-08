using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models;

public class ShoppingItem
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public Guid ShoppingListReminderId { get; set; }

    [Required]
    [ForeignKey(nameof(ShoppingListReminderId))]
    public ShoppingListReminder ShoppingListReminder { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public bool IsChecked { get; set; } = false;
}


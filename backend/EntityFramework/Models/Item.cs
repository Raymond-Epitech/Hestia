using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models;

public class Item
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public decimal Price { get; set; }

    [Required]
    public Guid ShoppingListId { get; set; }

    [ForeignKey("ShoppingListId")]
    public ShoppingList ShoppingList { get; set; } = null!;
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models;

public class ShoppingList
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public Guid ColocationId { get; set; }

    [ForeignKey("ColocationId")]
    public Colocation Colocation { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;
}

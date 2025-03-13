using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models;

public class Reminder
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid ColocationId { get; set; }

    [ForeignKey(nameof(ColocationId))]
    public Colocation Colocation { get; set; } = null!;

    [Required]
    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Required]
    public string Content { get; set; } = null!;

    [Required]
    public string Color { get; set; } = null!;

    public int CoordX { get; set; }

    public int CoordY { get; set; }

    public int CoordZ { get; set; }
}
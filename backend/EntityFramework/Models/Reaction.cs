using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models;

public class Reaction
{
    [Key]
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [Required]
    public Guid ReminderId { get; set; }

    [Required]
    [ForeignKey(nameof(ReminderId))]
    public Reminder Reminder { get; set; } = null!;

    public string Type { get; set; } = null!;
}


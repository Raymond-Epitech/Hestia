using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models;

public class Message
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid ColocationId { get; set; }

    [ForeignKey(nameof(ColocationId))]
    public Colocation Colocation { get; set; } = null!;

    [Required]
    public Guid SentBy { get; set; }

    [Required]
    [ForeignKey(nameof(SentBy))]
    public User User { get; set; } = null!;

    [Required]
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string Content { get; set; } = null!;

}

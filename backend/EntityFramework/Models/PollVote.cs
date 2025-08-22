using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models;

public class PollVote
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid PollReminderId { get; set; }

    [Required]
    [ForeignKey(nameof(PollReminder))]
    public PollReminder PollReminder { get; set; } = null!;

    [Required]
    public Guid VotedBy { get; set; }

    [Required]
    [ForeignKey(nameof(VotedBy))]
    public User User { get; set; } = null!;

    public DateTime VotedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string Choice { get; set; } = null!;
}


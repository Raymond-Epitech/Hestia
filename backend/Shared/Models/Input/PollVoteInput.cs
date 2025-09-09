using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models.Input;

public class PollVoteInput
{
    [Required]
    public Guid ReminderId { get; set; }

    [Required]
    public Guid VotedBy { get; set; }

    public DateTime VotedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string Choice { get; set; } = null!;
}


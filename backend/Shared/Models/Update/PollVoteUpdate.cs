using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Update;

public class PollVoteUpdate
{
    [Required]
    public Guid Id { get; set; }

    public DateTime VotedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string Choice { get; set; } = null!;
}


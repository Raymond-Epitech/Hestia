namespace Shared.Models.Output;

public class PollVoteOutput
{
    public Guid Id { get; set; }
    public Guid VotedBy { get; set; }
    public DateTime VotedAt { get; set; }
    public string Choice { get; set; } = null!;
}


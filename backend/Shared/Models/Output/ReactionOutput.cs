namespace Shared.Models.Output;

public class ReactionOutput
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ReminderId { get; set; }
    public string Type { get; set; } = null!;
}


namespace Shared.Models.Input;

public class ReactionInput
{
    public Guid ReminderId { get; set; }
    public Guid UserId { get; set; }
    public string Type { get; set; } = null!;
}


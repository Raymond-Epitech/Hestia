using Shared.Enums;

namespace Shared.Models.Output;

public class ReminderOutput
{
    public Guid Id { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public ReminderType ReminderType { get; set; }
    public string LinkToPP { get; set; } = null!;
    public int CoordX { get; set; }
    public int CoordY { get; set; }
    public int CoordZ { get; set; }
    public List<ReactionOutput> Reactions { get; set; } = new List<ReactionOutput>();
    public string? Content { get; set; } = null;
    public string? Color { get; set; } = null;
    public string? ImageUrl { get; set; } = null;
    public string? ShoppingListName { get; set; } = null;
    public List<ShoppingItemOutput>? Items { get; set; } = null;
    public string? Title { get; set; } = null;
    public string? Description { get; set; } = null;
    public DateTime? ExpirationDate { get; set; } = null;
    public bool? IsAnonymous { get; set; } = null;
    public bool? AllowMultipleChoices { get; set; } = null;
    public List<PollVoteOutput>? Votes { get; set; } = null;
}

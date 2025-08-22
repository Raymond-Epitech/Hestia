namespace Shared.Models.Output;

public abstract class ReminderOutput
{
    public Guid Id { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string LinkToPP { get; set; } = null!;
    public int CoordX { get; set; }
    public int CoordY { get; set; }
    public int CoordZ { get; set; }
}

public class TextReminderOutput : ReminderOutput
{
    public string Content { get; set; } = null!;
    public string Color { get; set; } = null!;
}

public class ImageReminderOutput : ReminderOutput
{
    public string ImageUrl { get; set; } = null!;
}

public class ShoppingListReminderOutput : ReminderOutput
{
    public string Name { get; set; } = null!;
    public List<ShoppingItemOutput> Items { get; set; } = new();
}

public class PollReminderOutput : ReminderOutput
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; } = null;
    public DateTime ExpirationDate { get; set; }
    public bool IsAnonymous { get; set; }
    public bool AllowMultipleChoices { get; set; }
    public List<PollVoteOutput> Votes { get; set; } = new();
}

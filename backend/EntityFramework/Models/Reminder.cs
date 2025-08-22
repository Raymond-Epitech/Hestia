using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models;

public abstract class Reminder
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid ColocationId { get; set; }

    [ForeignKey(nameof(ColocationId))]
    public Colocation Colocation { get; set; } = null!;

    [Required]
    public Guid CreatedBy { get; set; }

    [Required]
    [ForeignKey(nameof(CreatedBy))]
    public User User { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int CoordX { get; set; }

    public int CoordY { get; set; }

    public int CoordZ { get; set; }

    public ICollection<Reaction> Reactions { get; set; } = null!;
}

public class TextReminder : Reminder
{
    [Required]
    public string Content { get; set; } = null!;

    [Required]
    public string Color { get; set; } = null!;
}

public class ImageReminder : Reminder
{
    [Required]
    public string ImageUrl { get; set; } = null!;
}

public class ShoppingListReminder : Reminder
{
    [Required]
    public string ShoppingListName { get; set; } = null!;

    public ICollection<ShoppingItem> ShoppingItems { get; set; } = null!;
}

public class PollReminder : Reminder
{
    [Required]
    public string Title { get; set; } = null!;

    public string? Description { get; set; } = null;

    public DateTime ExpirationDate { get; set; } = DateTime.UtcNow.AddDays(1);

    public bool IsAnonymous { get; set; } = false;

    public bool AllowMultipleChoices { get; set; } = false;

    public ICollection<PollVote> PollVotes { get; set; } = null!;
}
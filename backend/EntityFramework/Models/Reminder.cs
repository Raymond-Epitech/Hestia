namespace EntityFramework.Models;

public class Reminder : IEntity
{
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string UpdatedBy { get; set; } = null!;
    public DateTime UpdatedAt { get; set; }
    public Guid Id { get; set; }
    public string Content { get; set; } = null!;
    public string Color { get; set; } = null!;
}
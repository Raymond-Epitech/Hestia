namespace EntityFramework.Models;

public class Reminder
{
    public Guid Id { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string Content { get; set; } = null!;
    public string Color { get; set; } = null!;
}
namespace Shared.Models.Output;

public class MessageOutput
{
    public Guid Id { get; set; }
    public Guid ColocationId { get; set; }
    public string Content { get; set; } = null!;
    public Guid SendBy { get; set; }
    public DateTime SendAt { get; set; }
}

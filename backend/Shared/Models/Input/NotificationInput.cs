namespace Shared.Models.Input;

public class NotificationInput
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
}


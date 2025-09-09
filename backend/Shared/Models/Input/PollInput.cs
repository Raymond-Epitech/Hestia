namespace Shared.Models.Input;

public class PollInput
{
    public string? Title { get; set; } = null;
    public string? Description { get; set; } = null;
    public DateTime? ExpirationDate { get; set; } = DateTime.UtcNow.AddDays(1);
    public bool? IsAnonymous { get; set; } = false;
    public bool? AllowMultipleChoices { get; set; } = false;
}


using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Input;

public class PollInput
{
    [Required]
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime ExpirationDate { get; set; } = DateTime.UtcNow.AddDays(1);
    public bool IsAnonymous { get; set; } = false;
    public bool AllowMultipleChoices { get; set; } = false;
}


using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Input;

public class MessageInput
{
    [Required]
    public Guid ColocationId { get; set; }

    [Required]
    public string Content { get; set; } = null!;

    [Required]
    public Guid SendBy { get; set; }
}


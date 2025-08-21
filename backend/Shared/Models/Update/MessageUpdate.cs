using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Update;

public class MessageUpdate
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid ColocationId { get; set; }

    [Required]
    public string Content { get; set; } = null!;

    [Required]
    public Guid SendBy { get; set; }
}


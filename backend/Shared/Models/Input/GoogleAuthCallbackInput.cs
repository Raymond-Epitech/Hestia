using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Input;

public class GoogleAuthCallbackInput
{
    [Required]
    public string Code { get; set; } = null!;
}


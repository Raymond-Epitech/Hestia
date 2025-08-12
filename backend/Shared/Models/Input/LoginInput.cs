using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Input;

public class LoginInput
{
    [Required]
    public string GoogleToken { get; set; } = null!;

    public string? FCMToken { get; set; } = null;
}

using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Input;

public class LanguageInput
{
    [Required]
    public Guid UserId { get; set; }

    public string Language { get; set; } = "fr";
}


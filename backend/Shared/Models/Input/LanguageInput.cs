using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Input;

public class LanguageInput
{
    [Required]
    public Guid UserId { get; set; }

    public Languages Language { get; set; } = Languages.English;
}


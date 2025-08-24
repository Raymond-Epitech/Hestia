using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Input
{
    public class ReminderInput
    {
        [Required]
        public Guid ColocationId { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }

        public string? Content { get; set; } = null;

        public bool IsImage { get; set; } = false;

        public IFormFile? Image { get; set; } = null;

        [Required]
        public string Color { get; set; } = null!;

        public int CoordX { get; set; } = 0;

        public int CoordY { get; set; } = 0;

        public int CoordZ { get; set; } = 0;
    }
}

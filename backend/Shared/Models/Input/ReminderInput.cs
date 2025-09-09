using Microsoft.AspNetCore.Http;
using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Input
{
    public class ReminderInput
    {
        [Required]
        public Guid ColocationId { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }

        public int CoordX { get; set; } = 0;

        public int CoordY { get; set; } = 0;

        public int CoordZ { get; set; } = 0;
        
        [Required]
        public ReminderType ReminderType { get; set; } 

        public string? Content { get; set; } = null;

        public string? Color { get; set; } = null;

        public IFormFile? File { get; set; } = null;

        public string? ShoppingListName { get; set; } = null;

        public PollInput? PollInput { get; set; } = null;
    }
}

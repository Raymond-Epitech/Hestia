using Microsoft.AspNetCore.Http;
using Shared.Enums;
using Shared.Models.Input;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Update
{
    public class ReminderUpdate
    {
        [Required]
        public Guid Id { get; set; }

        public int CoordX { get; set; } = 0;

        public int CoordY { get; set; } = 0;

        public int CoordZ { get; set; } = 0;
        
        [Required]
        public ReminderType ReminderType { get; set; }

        public string? Content { get; set; } = null;

        public string? Color { get; set; } = null;

        public string? ShoppingListName { get; set; } = null;

        public PollInput? PollInput { get; set; } = null;
    }
}
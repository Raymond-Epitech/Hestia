using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Update
{
    public class ReminderUpdate
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Content { get; set; } = null!;

        [Required]
        public string Color { get; set; } = null!;

        public int CoordX { get; set; } = 0;

        public int CoordY { get; set; } = 0;
        public int CoordZ { get; set; } = 0;
    }
}
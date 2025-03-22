using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Output
{
    public class ChoreInput
    {
        [Required]
        public Guid ColocationId { get; set; }

        [Required]
        public string CreatedBy { get; set; } = null!;

        public DateTime DueDate { get; set; } = DateTime.Now.ToUniversalTime();

        [Required]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsDone { get; set; } = false;
    }
}

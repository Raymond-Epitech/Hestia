using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Output
{
    public class ChoreInput
    {
        [Required]
        public Guid ColocationId { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }
        public DateTime DueDate { get; set; } = DateTime.Now.ToUniversalTime();

        [Required]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsDone { get; set; } = false;
        public List<Guid>? Enrolled { get; set; } = null;
    }
}

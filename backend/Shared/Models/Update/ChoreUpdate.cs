using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Update
{
    public class ChoreUpdate
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid ColocationId { get; set; }

        public DateTime DueDate { get; set; } = DateTime.Now.ToUniversalTime();

        [Required]
        public string Title { get; set; } = null!;

        public string? Description { get; set; } = null;

        public bool IsDone { get; set; } = false;
    }
}

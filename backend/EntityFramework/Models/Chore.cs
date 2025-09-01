using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    public class Chore
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }

        [Required]
        [ForeignKey(nameof(CreatedBy))]
        public User User { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid ColocationId { get; set; }

        [ForeignKey(nameof(ColocationId))]
        public Colocation Colocation { get; set; } = null!;

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool IsDone { get; set; } = false;

        public ICollection<ChoreEnrollment> ChoreEnrollments { get; set; } = null!;
    }
}

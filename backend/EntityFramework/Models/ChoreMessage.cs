using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    public class ChoreMessage : IEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public Guid ChoreId { get; set; }

        [ForeignKey(nameof(ChoreId))]
        public Chore Chore { get; set; } = null!;

        [Required]
        public string Content { get; set; } = null!;
    }
}

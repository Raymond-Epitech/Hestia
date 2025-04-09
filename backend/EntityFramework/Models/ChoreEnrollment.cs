using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    public class ChoreEnrollment
    {
        [Required]
        public Guid ChoreId { get; set; }

        [ForeignKey(nameof(ChoreId))]
        public Chore Chore { get; set; } = null!;

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    public class Balance
    {
        [Key]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [Column(TypeName = "decimal(19,2)")]
        public decimal PersonalBalance { get; set; }

        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
    }
}

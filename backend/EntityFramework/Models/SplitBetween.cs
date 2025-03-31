using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    public class SplitBetween
    {
        [Required]
        public Guid ExpenseId { get; set; }

        [ForeignKey("ExpenseId")]
        public Expense Expense { get; set; } = null!;

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}

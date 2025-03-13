using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    public class Expense
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        [Column(TypeName = "decimal(19,2)")]
        public decimal Amount { get; set; }
        DateTime DateOfPayment { get; set; }
        public Guid ColocationID { get; set; }
        public Colocation Colocation { get; set; } = null!;
    }
}

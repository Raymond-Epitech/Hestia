using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    public class Expense
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string CreatedBy { get; set; } = null!;

        [Required]
        public Guid ColocationId { get; set; }

        [ForeignKey("Colocation")]
        public Colocation Colocation { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Column(TypeName = "decimal(19,2)")]
        public decimal Amount { get; set; }

        [Required]
        public Guid PaidBy { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string SplitBetweenJson { get; set; } = "[]";

        [NotMapped]
        public List<Guid> SplitBetween
        {
            get => JsonConvert.DeserializeObject<List<Guid>>(SplitBetweenJson) ?? new List<Guid>();
            set => SplitBetweenJson = JsonConvert.SerializeObject(value);
        }

        [Required]
        public string SplitType { get; set; } = "Evenly";

        [Required]
        public DateTime DateOfPayment { get; set; }

        public ICollection<Entry> Entries { get; set; } = null!;
    }
}

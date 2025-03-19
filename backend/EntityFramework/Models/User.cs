using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime LastConnection { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid? ColocationId { get; set; } = null;

        [ForeignKey(nameof(ColocationId))]
        public Colocation? Colocation { get; set; } = null;

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string PathToProfilePicture { get; set; } = null!;

        public ICollection<ChoreEnrollment> ChoreEnrollments { get; set; } = null!;
        public ICollection<Balance> Balances { get; set; } = null!;
        public ICollection<Entry> Entries { get; set; } = null!;
        public ICollection<Expense> Expenses { get; set; } = null!;
        public ICollection<SplitBetween> SplitBetweens { get; set; } = null!;
    }
}

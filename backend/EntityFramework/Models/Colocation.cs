using System.ComponentModel.DataAnnotations;

namespace EntityFramework.Models
{
    public class Colocation
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid CreatedBy { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;

        public ICollection<User> Users { get; set; } = null!;
        public ICollection<Chore> Chores { get; set; } = null!;
        public ICollection<Reminder> Reminders { get; set; } = null!;
        public ICollection<Expense> Expenses { get; set; } = null!;
        public ICollection<ShoppingList> ShoppingList { get; set; } = null!;
    }
}

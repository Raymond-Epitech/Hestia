using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid? ColocationId { get; set; }

        [ForeignKey(nameof(ColocationId))]
        public Colocation? Colocation { get; set; }

        public DateTime LastConnection { get; set; } = DateTime.UtcNow;

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string PathToProfilePicture { get; set; } = null!;

        public bool IsDeleted { get; set; } = false;

        public ICollection<ChoreEnrollment> ChoreEnrollments { get; set; } = null!;
        public ICollection<Entry> Entries { get; set; } = null!;
        public ICollection<Expense> Expenses { get; set; } = null!;
        public ICollection<SplitBetween> SplitBetweens { get; set; } = null!;
        public ICollection<FCMDevice> FCMDevices { get; set; } = null!;
        public ICollection<Reminder> Reminders { get; set; } = null!;
        public ICollection<Chore> Chores { get; set; } = null!;
        public ICollection<ChoreMessage> ChoreMessages { get; set; } = null!;
        public ICollection<Message> Messages { get; set; } = null!;
    }
}

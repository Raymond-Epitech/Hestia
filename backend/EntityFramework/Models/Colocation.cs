namespace EntityFramework.Models
{
    public class Colocation
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public ICollection<User> Users { get; set; } = null!;
        public ICollection<Chore> Chores { get; set; } = null!;
        public ICollection<Reminder> Reminders { get; set; } = null!;
    }
}

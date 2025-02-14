namespace EntityFramework.Models
{
    public class ChoreEnrollment
    {
        public Guid ChoreId { get; set; }
        public Chore Chore { get; set; } = null!;
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}

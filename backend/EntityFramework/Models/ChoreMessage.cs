namespace EntityFramework.Models
{
    public class ChoreMessage
    {
        public Guid Id { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public Guid ChoreId { get; set; }
        public Chore Chore { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
}

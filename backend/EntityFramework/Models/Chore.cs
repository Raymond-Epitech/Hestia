namespace EntityFramework.Models
{
    public class Chore
    {
        public Guid Id { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime DueDate { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsDone { get; set; }
        public ICollection<ChoreMessage> ChoreMessages { get; set; } = null!;
    }
}

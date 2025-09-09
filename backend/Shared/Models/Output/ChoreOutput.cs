namespace Shared.Models.Output
{
    public class ChoreOutput
    {
        public Guid Id { get; set; }
        public Guid CreatedBy { get; set; }
        public Dictionary<Guid, string> EnrolledUsers { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DueDate { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsDone { get; set; }
    }
}

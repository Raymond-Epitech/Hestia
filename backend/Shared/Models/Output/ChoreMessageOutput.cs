namespace Shared.Models.Output
{
    public class ChoreMessageOutput
    {
        public Guid Id { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; } = null!;
    }
}

namespace Shared.Models.Output
{
    public class ChoreMessageOutput
    {
        public Guid Id { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; } = null!;
    }
}

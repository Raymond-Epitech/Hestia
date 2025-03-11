namespace Shared.Models.Input
{
    public class ChoreMessageInput
    {
        public Guid ChoreId { get; set; }
        public string CreatedBy { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
}

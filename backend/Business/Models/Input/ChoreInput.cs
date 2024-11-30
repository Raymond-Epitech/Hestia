namespace Business.Models.Output
{
    public class ChoreInput
    {
        public string CreatedBy { get; set; } = null!;
        public DateTime DueDate { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsDone { get; set; }
    }
}

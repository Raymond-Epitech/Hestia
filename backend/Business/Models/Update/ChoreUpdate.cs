namespace Business.Models.Update
{
    public class ChoreUpdate
    {
        public Guid Id { get; set; }
        public Guid CollocationId { get; set; }
        public DateTime DueDate { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsDone { get; set; }
    }
}

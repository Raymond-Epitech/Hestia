namespace Business.Models.Output
{
    public class ReminderOutput
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = null!;
        public string Color { get; set; } = null!;
    }
}

namespace Business.Models.Input
{
    public class ReminderUpdate
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = null!;
        public string Color { get; set; } = null!;
    }
}
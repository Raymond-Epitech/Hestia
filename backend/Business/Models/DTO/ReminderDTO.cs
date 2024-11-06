namespace Business.Models.DTO
{
    public class ReminderDTO
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = null!;
        public string Color { get; set; } = null!;
    }
}

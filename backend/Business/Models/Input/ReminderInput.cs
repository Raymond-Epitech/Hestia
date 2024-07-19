namespace Business.Models.Input
{
    public class ReminderInput
    {
        public Guid? Id { get; set; }
        public string CreatedBy { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string Color { get; set; } = null!;
    }
}

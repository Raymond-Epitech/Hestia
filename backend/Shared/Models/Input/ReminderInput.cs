namespace Shared.Models.Input
{
    public class ReminderInput
    {
        public Guid ColocationId { get; set; }
        public string CreatedBy { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string Color { get; set; } = null!;
        public int CoordX { get; set; }
        public int CoordY { get; set; }
        public int CoordZ { get; set; }
    }
}

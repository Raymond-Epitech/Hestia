namespace Shared.Models.Update
{
    public class ReminderUpdate
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = null!;
        public string Color { get; set; } = null!;
        public int CoordX { get; set; }
        public int CoordY { get; set; }
        public int CoordZ { get; set; }
    }
}
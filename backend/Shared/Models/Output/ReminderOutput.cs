﻿namespace Shared.Models.Output
{
    public class ReminderOutput
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = null!;
        public string Color { get; set; } = null!;
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CoordX { get; set; }
        public int CoordY { get; set; }
        public int CoordZ { get; set; }
    }
}

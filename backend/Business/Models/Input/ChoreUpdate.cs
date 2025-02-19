﻿namespace Business.Models.Output
{
    public class ChoreUpdate
    {
        public Guid Id { get; set; }
        public DateTime DueDate { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsDone { get; set; }
    }
}

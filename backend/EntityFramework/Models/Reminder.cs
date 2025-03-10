﻿namespace EntityFramework.Models;

public class Reminder
{
    public Guid Id { get; set; }
    public Guid CollocationId { get; set; }
    public Collocation Collocation { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string Content { get; set; } = null!;
    public string Color { get; set; } = null!;
    public int CoordX { get; set; }
    public int CoordY { get; set; }
    public int CoordZ { get; set; }
}
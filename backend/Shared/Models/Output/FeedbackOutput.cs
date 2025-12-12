using Shared.Enums;

namespace Shared.Models.Input;

public class FeedbackOutput
{
    public Guid Id { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string VersionHash { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public BugType BugType { get; set; }
    public string? AttachmentPath { get; set; } = null;
}


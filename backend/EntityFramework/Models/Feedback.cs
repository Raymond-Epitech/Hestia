using Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models;

public class Feedback
{
    [Key]
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid CreatedBy { get; set; }

    [Required]
    [ForeignKey("CreatedBy")]
    public User User { get; set; } = null!;

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public string VersionHash { get; set; } = null!;

    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    public BugType BugType { get; set; } = BugType.BugOrError;

    public string? AttachmentPath { get; set; } = null;
}


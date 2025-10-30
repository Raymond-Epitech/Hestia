using Microsoft.AspNetCore.Http;
using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Input;

public class FeedbackInput
{
    [Required]
    public Guid CreatedBy { get; set; }

    [Required]
    public string VersionHash { get; set; } = null!;

    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;
    
    public BugType BugType { get; set; } = BugType.BugOrError;

    [Required]
    public IFormFile Attatchment { get; set; } = null!;
}


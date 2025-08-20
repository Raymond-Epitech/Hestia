using System.ComponentModel.DataAnnotations;

namespace EntityFramework.Models;

public class FCMDevice
{
    [Required]
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    [Key]
    public string FCMToken { get; set; } = null!;
}


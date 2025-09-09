using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models;

public class FCMDevice
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string FCMToken { get; set; } = null!;

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}


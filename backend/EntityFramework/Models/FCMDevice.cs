using System.ComponentModel.DataAnnotations;

namespace EntityFramework.Models;

public class FCMDevice
{
    [Key]
    public string FCMToken { get; set; } = null!;

    public ICollection<User> Users { get; set; } = null!;
}


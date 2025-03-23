using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Input
{
    public class UserInput
    {
        [Required]
        public string Username { get; set; } = null!;

        public Guid ColocationId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Update
{
    public class UserUpdate
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string PathToProfilePicture { get; set; } = null!;

        public Guid? ColocationId { get; set; }
    }
}

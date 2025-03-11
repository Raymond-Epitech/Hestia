namespace Shared.Models.Update
{
    public class UserUpdate
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PathToProfilePicture { get; set; } = null!;
        public Guid? ColocationId { get; set; }
    }
}

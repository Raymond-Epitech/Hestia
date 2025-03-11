namespace Shared.Models.Output
{
    public class UserOutput
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public Guid? ColocationId { get; set; }
    }
}

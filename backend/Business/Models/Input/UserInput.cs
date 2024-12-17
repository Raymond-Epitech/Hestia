namespace Business.Models.Input
{
    public class UserInput
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public Guid? CollocationId { get; set; }
    }
}

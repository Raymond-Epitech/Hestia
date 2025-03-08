namespace EntityFramework.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastConnection { get; set; }
        public Guid? ColocationId { get; set; }
        public Colocation Colocation { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PathToProfilePicture { get; set; } = null!;
        public ICollection<ChoreEnrollment> ChoreEnrollments { get; set; } = null!;
    }
}

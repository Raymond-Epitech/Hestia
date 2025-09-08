namespace Shared.Models.Output
{
    public class ChoreEnrollmentOutput
    {
        public Guid UserId { get; set; }
        public Guid ChoreId { get; set; }
        public string PathToPP { get; set; } = null!;
    }
}

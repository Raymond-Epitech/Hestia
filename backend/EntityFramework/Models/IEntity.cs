namespace EntityFramework.Models
{
    public interface IEntity
    {
        Guid Id { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

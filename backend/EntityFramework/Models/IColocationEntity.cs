namespace EntityFramework.Models;

public interface IColocationEntity : IEntity
{
    public Guid ColocationId { get; set; }
    public Colocation Colocation { get; set; }
}

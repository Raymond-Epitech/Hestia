namespace Shared.Models.Output
{
    public class ColocationOutput
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public List<Guid>? Colocataires { get; set; }
    }
}

namespace Business.Models.Output
{
    public class CollocationOutput
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public List<Guid>? Collocataires { get; set; }
    }
}

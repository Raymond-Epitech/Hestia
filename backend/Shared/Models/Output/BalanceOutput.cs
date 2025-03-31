namespace Shared.Models.Output
{
    public class BalanceOutput
    {
        public Guid UserId { get; set; }
        public decimal PersonalBalance { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}

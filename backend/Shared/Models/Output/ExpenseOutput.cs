using Shared.Enums;

namespace Shared.Models.Output
{
    public class ExpenseOutput
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid ColocationId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public Guid PaidBy { get; set; }
        public Dictionary<Guid, decimal> SplitBetween { get; set; } = new Dictionary<Guid, decimal>();
        public SplitTypeEnum SplitType { get; set; }
        public DateTime DateOfPayment { get; set; }
        public string Category { get; set; } = null!;
    }
}

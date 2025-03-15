using Shared.Enums;

namespace Shared.Models.Input
{
    public class ExpenseInput()
    {
        public Guid ColocationId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public Guid PaidBy { get; set; }
        public SplitTypeEnum SplitType { get; set; }
        public List<Guid>? SplitBetween { get; set; }
        public Dictionary<Guid, decimal>? SplitValues { get; set; }
        public Dictionary<Guid, int>? SplitPercentages { get; set; }
        public DateTime DateOfPayment { get; set; }
    }
}

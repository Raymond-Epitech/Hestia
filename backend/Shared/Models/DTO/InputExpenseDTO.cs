using Shared.Enums;

namespace Shared.Models.DTO
{
    public class InputExpenseDTO
    {
        public Guid Id { get; set; }
        public SplitTypeEnum SplitType { get; set; } = SplitTypeEnum.Evenly;
        public List<Guid>? SplitBetween { get; set; } = null;
        public Dictionary<Guid, decimal>? SplitValues { get; set; } = null;
        public Dictionary<Guid, int>? SplitPercentages { get; set; } = null;
        public decimal Amount { get; set; }
        public Guid ColocationId { get; set; }
        public Guid PaidBy { get; set; }
    }
}

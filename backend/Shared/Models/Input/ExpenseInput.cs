using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Input
{
    public class ExpenseInput()
    {
        [Required]
        public Guid ColocationId { get; set; }

        [Required]
        public string CreatedBy { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; } = null;

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public Guid PaidBy { get; set; }

        public SplitTypeEnum SplitType { get; set; } = SplitTypeEnum.Evenly;

        public List<Guid>? SplitBetween { get; set; } = null;

        public Dictionary<Guid, decimal>? SplitValues { get; set; } = null;

        public Dictionary<Guid, int>? SplitPercentages { get; set; } = null;

        public DateTime DateOfPayment { get; set; } = DateTime.Now.ToUniversalTime();
    }
}

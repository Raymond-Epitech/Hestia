namespace Shared.Models.DTO
{
    public class ExpenseDTO
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = null!;
        public Guid ColocationId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string PaidBy { get; set; } = null!;
        public List<string> SplitBetween { get; set; } = new List<string>();
        public string SplitType { get; set; } = null!;
        public DateTime DateOfPayment { get; set; }
    }
}

namespace Shared.Models.Output
{
    public class OutputFormatForExpenses
    {
        public string Category { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public List<ExpenseOutput> Expenses { get; set; } = null!;
    }
}

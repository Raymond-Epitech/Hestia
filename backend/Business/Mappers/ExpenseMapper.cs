using EntityFramework.Models;
using Shared.Models.Output;

namespace Business.Mappers
{
    public static class ExpenseMapper
    {
        public static List<OutputFormatForExpenses> ToOutputFormat(this List<ExpenseOutput> expenses)
        {
            return expenses.GroupBy(e => e.Category)
                .Select(e => new OutputFormatForExpenses
                {
                    Category = e.Key,
                    TotalAmount = e.Sum(x => x.Amount),
                    Expenses = e.ToList()
                }).ToList();
        }
    }
}

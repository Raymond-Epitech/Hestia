using EntityFramework.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shared.Enums;
using Shared.Models.DTO;
using Shared.Models.Output;

namespace Business.Mappers
{
    public static class ExpenseMapper
    {
        public static BalanceOutput ToOutput(this Balance balance)
        {
            return new BalanceOutput
            {
                UserId = balance.UserId,
                PersonalBalance = balance.PersonalBalance,
                LastUpdate = balance.LastUpdate
            };
        }

        public static List<OutputFormatForExpenses> ToOutputFormat(this List<ExpenseOutput> expenses)
        {
            return expenses.GroupBy(e => e.ShoppingListName)
                .Select(e => new OutputFormatForExpenses
                {
                    ShoppingListName = e.Key,
                    TotalAmount = e.Sum(x => x.Amount),
                    Expenses = e.ToList()
                }).ToList();
        }
    }
}

using EntityFramework.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shared.Enums;
using Shared.Models.DTO;
using Shared.Models.Output;

namespace Business.Mappers
{
    public static class ExpenseMapper
    {
        public static ExpenseOutput ToOutput(this ExpenseDTO expenseDTO)
        {
            return new ExpenseOutput
            {
                Id = expenseDTO.Id,
                CreatedAt = expenseDTO.CreatedAt,
                CreatedBy = expenseDTO.CreatedBy,
                ColocationId = expenseDTO.ColocationId,
                Name = expenseDTO.Name,
                Description = expenseDTO.Description,
                Amount = expenseDTO.Amount,
                PaidBy = expenseDTO.PaidBy,
                SplitBetween = expenseDTO.SplitBetween,
                SplitType = Enum.Parse<SplitTypeEnum>(expenseDTO.SplitType),
                DateOfPayment = expenseDTO.DateOfPayment
            };
        }

        public static BalanceOutput ToOutput(this Balance balance)
        {
            return new BalanceOutput
            {
                UserId = balance.UserId,
                PersonalBalance = balance.PersonalBalance,
                LastUpdate = balance.LastUpdate
            };
        }
    }
}

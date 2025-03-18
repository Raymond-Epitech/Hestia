using EntityFramework.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Shared.Models.DTO;

namespace EntityFramework.Repositories.Interfaces
{
    public interface IExpenseRepository
    {
        Task<List<ExpenseDTO>> GetAllExpensesDTOAsync(Guid ColocationId);
        Task<ExpenseDTO?> GetExpenseDTOAsync(Guid id);
        Task AddRangeEntryAsync(List<Entry> entryList);
        Task AddExpenseAsync(Expense expense);
        Task<List<Balance>> GetBalanceFromUserIdListAsync(List<Guid> userIds);
        Task UpdateRangeBalanceAsync(List<Balance> balances);
        Task AddRangeSplitBetweenAsync(List<SplitBetween> splitBetweenList);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task SaveChangesAsync();
    }
}

using EntityFramework.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Shared.Models.DTO;
using Shared.Models.Output;

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
        Task<List<BalanceOutput>> GetAllBalancesOutputFromColocationIdListAsync(Guid colocationId);
        Task<List<Balance>> GetAllBalancesFromColocationIdListAsync(Guid colocationId);

        Task<List<Entry>> GetAllEntriesFromColocationIdAsync(Guid colocationId);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task SaveChangesAsync();
    }
}

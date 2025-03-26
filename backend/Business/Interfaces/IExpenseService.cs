using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Interfaces
{
    public interface IExpenseService
    {
        Task<List<ExpenseOutput>> GetAllExpensesAsync(Guid ColocationId);
        Task<ExpenseOutput> GetExpenseAsync(Guid id);
        Task<Guid> AddExpenseAsync(ExpenseInput input);
        Task<Guid> UpdateExpenseAsync(ExpenseUpdate input);
        Task<Guid> DeleteExpenseAsync(Guid id);
        Task<List<BalanceOutput>> GetAllBalanceAsync(Guid colocationId);
        Task<List<BalanceOutput>> RecalculateBalanceAsync(Guid colocationId);
    }
}

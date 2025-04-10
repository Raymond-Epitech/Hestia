using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Interfaces
{
    public interface IExpenseService
    {
        Task<List<ExpenseCategoryOutput>> GetAllExpenseCategoriesAsync(Guid colocationId);
        Task<List<ExpenseOutput>> GetAllExpensesAsync(Guid expenseCategoryId);
        Task<ExpenseOutput> GetExpenseAsync(Guid id);
        Task<Guid> AddExpenseAsync(ExpenseInput input);
        Task<Guid> UpdateExpenseAsync(ExpenseUpdate input);
        Task<Guid> DeleteExpenseAsync(Guid id);
        Task<Dictionary<Guid, decimal>> GetAllBalanceAsync(Guid colocationId);
    }
}

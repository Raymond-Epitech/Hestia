using Shared.Models.Input;
using Shared.Models.Output;

namespace Business.Interfaces
{
    public interface IExpenseService
    {
        Task<List<ExpenseOutput>> GetAllExpensesAsync(Guid ColocationId);
        Task<ExpenseOutput> GetExpenseAsync(Guid id);
        Task<Guid> AddExpenseAsync(ExpenseInput input);
    }
}

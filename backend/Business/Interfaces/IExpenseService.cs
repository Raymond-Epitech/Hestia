using Shared.Models.Input;
using Shared.Models.Output;

namespace Business.Interfaces
{
    public interface IExpenseService
    {
        Task<List<ExpenseOutput>> GetAllExpenses(Guid ColocationId);
        Task<ExpenseOutput> GetExpenseAsync(Guid id);
        Task<Guid> AddExpenseAsync(ExpenseInput input);
    }
}

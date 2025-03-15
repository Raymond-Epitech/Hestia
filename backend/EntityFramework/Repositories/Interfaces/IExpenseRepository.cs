using Microsoft.EntityFrameworkCore.Storage;
using Shared.Models.DTO;
using Shared.Models.Output;

namespace EntityFramework.Repositories.Interfaces
{
    public interface IExpenseRepository
    {
        Task<List<ExpenseDTO>> GetAllExpensesDTOAsync(Guid ColocationId);
        Task<ExpenseDTO?> GetExpenseDTOAsync(Guid id);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task SaveChangesAsync();
    }
}

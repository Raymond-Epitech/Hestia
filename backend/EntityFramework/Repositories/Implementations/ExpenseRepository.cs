using EntityFramework.Context;
using EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shared.Models.DTO;
using Shared.Models.Output;

namespace EntityFramework.Repositories.Implementations
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly HestiaContext _context;

        public ExpenseRepository(HestiaContext context)
        {
            _context = context;
        }

        public async Task<List<ExpenseDTO>> GetAllExpensesDTOAsync(Guid ColocationId)
        {
            var expenses = await _context.Expenses.Where(x => x.ColocationId == ColocationId).Select(x => new ExpenseDTO
            {
                Id = x.Id,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                ColocationId = x.ColocationId,
                Name = x.Name,
                Description = x.Description,
                Amount = x.Amount,
                PaidBy = x.PaidBy,
                SplitBetween = x.SplitBetween,
                SplitType = x.SplitType,
                DateOfPayment = x.DateOfPayment
            }).ToListAsync();

            return expenses;
        }

        public async Task<ExpenseDTO?> GetExpenseDTOAsync(Guid id)
        {
            return await _context.Expenses.Where(x => x.Id == id).Select(x => new ExpenseDTO
            {
                Id = x.Id,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                ColocationId = x.ColocationId,
                Name = x.Name,
                Description = x.Description,
                Amount = x.Amount,
                PaidBy = x.PaidBy,
                SplitBetween = x.SplitBetween,
                SplitType = x.SplitType,
                DateOfPayment = x.DateOfPayment
            }).FirstOrDefaultAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

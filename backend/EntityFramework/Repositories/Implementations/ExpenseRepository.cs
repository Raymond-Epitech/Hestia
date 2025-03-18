using EntityFramework.Context;
using EntityFramework.Models;
using EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shared.Models.DTO;

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
                PaidBy = x.User.Username,
                SplitBetween = x.SplitBetweens.Select(y => y.User.Username).ToList(),
                SplitType = x.SplitType,
                DateOfPayment = x.DateOfPayment
            }).ToListAsync();

            return expenses;
        }

        public async Task<ExpenseDTO?> GetExpenseDTOAsync(Guid id)
        {
            return await _context.Expenses.Where(x => x.Id == id)
                .Select(x => new ExpenseDTO
            {
                Id = x.Id,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                ColocationId = x.ColocationId,
                Name = x.Name,
                Description = x.Description,
                Amount = x.Amount,
                PaidBy = x.User.Username,
                SplitBetween = x.SplitBetweens.Select(y => y.User.Username).ToList(),
                SplitType = x.SplitType,
                DateOfPayment = x.DateOfPayment
            }).FirstOrDefaultAsync();
        }

        public async Task AddRangeEntryAsync(List<Entry> entryList)
        {
            await _context.Entries.AddRangeAsync(entryList);
        }

        public async Task AddExpenseAsync(Expense expense)
        {
            await _context.Expenses.AddAsync(expense);
        }

        public async Task<List<Balance>> GetBalanceFromUserIdListAsync(List<Guid> userIds)
        {
            return await _context.Balances.Where(x => userIds.Contains(x.UserId)).ToListAsync();
        }

        public async Task UpdateRangeBalanceAsync(List<Balance> balances)
        {
            _context.Balances.UpdateRange(balances);
        }

        public async Task AddRangeSplitBetweenAsync(List<SplitBetween> splitBetweenList)
        {
            await _context.SplitBetweens.AddRangeAsync(splitBetweenList);
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

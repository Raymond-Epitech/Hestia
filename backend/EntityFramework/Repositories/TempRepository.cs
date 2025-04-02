using EntityFramework.Context;
using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;
using Shared.Models.Output;

namespace EntityFramework.Repositories
{
    public class TempRepository(HestiaContext context,
        ILogger<TempRepository> logger
        ) : ITempRepository
    {
        public async Task<List<ChoreMessageOutput>> GetAllChoreMessageOutputByChoreIdAsync(Guid choreId)
        {
            var choreMessages = await context.ChoreMessages
                .Where(c => c.ChoreId == choreId)
                .Select(c => new ChoreMessageOutput
                {
                    Id = c.Id,
                    CreatedBy = c.CreatedBy,
                    CreatedAt = c.CreatedAt,
                    Content = c.Content,
                })
                .AsNoTracking()
                .ToListAsync();
            logger.LogInformation($"Succes : All chore messages from the chore {choreId} found");
            return choreMessages;
        }

        public async Task<int> DeleteRangeChoreMessageFromChoreId(Guid choreId)
        {
            var choreMessages = await context.ChoreMessages
                .Where(c => c.ChoreId == choreId)
                .AsNoTracking()
                .ToListAsync();
            context.ChoreMessages.RemoveRange(choreMessages);
            return choreMessages.Count;
        }

        public async Task<List<UserOutput>> GetEnrolledUserOutputFromChoreIdAsync(Guid choreId)
        {
            var users = await context.Users.Where(u => u.ChoreEnrollments.Any(ce => ce.ChoreId == choreId))
                .Select(u => new UserOutput
                {
                    Id = u.Id,
                    Email = u.Email,
                    Username = u.Username,
                    ColocationId = u.ColocationId
                })
                .AsNoTracking()
                .ToListAsync();
            logger.LogInformation($"Succes : All enrolled users from the chore {choreId} found");
            return users;
        }

        public async Task<List<ChoreOutput>> GetEnrolledChoreOutputFromUserIdAsync(Guid userId)
        {
            var chores = await context.Chores.Where(c => c.ChoreEnrollments.Any(ce => ce.UserId == userId))
                .Select(c => new ChoreOutput
                {
                    Id = c.Id,
                    CreatedBy = c.CreatedBy,
                    CreatedAt = c.CreatedAt,
                    DueDate = c.DueDate,
                    Title = c.Title,
                    Description = c.Description,
                    IsDone = c.IsDone
                })
                .AsNoTracking()
                .ToListAsync();
            logger.LogInformation($"Succes : All enrolled chores from the user {userId} found");
            return chores;
        }

        public async Task RemoveChoreEnrollmentAsync(Guid userId, Guid choreId)
        {
            var choreEnrollment = await context.ChoreEnrollments
                .Where(ce => ce.UserId == userId && ce.ChoreId == choreId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (choreEnrollment == null)
            {
                throw new NotFoundException($"Enrollment for chore {choreId} of user {userId} not found");
            }

            context.ChoreEnrollments.Remove(choreEnrollment);
        }

        public async Task<List<Balance>> GetBalanceFromUserIdListAsync(List<Guid> userList)
        {
            var balances = await context.Balances
                .Where(b => userList.Contains(b.UserId))
                .AsNoTracking()
                .ToListAsync();
            return balances;
        }

        public async Task DeleteAllEntriesByExpenseId(Guid expenseId)
        {
            var entries = await context.Entries
                .Where(e => e.ExpenseId == expenseId)
                .AsNoTracking()
                .ToListAsync();
            context.Entries.RemoveRange(entries);
        }

        public async Task<List<BalanceOutput>> GetAllByColocationIdAsync(Guid colocationId)
        {
            return await context.Balances
                .Where(b => b.User.ColocationId == colocationId)
                .Select(b => new BalanceOutput
                {
                    UserId = b.UserId,
                    PersonalBalance = b.PersonalBalance,
                    LastUpdate = b.LastUpdate
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Balance>> GetAllBalancesFromColocationIdListAsync(Guid colocationId)
        {
            return await context.Balances
                .Where(b => b.User.ColocationId == colocationId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> AnyExistingUserByEmail(string email)
        {
            return await context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<UserOutput>> GetAllByColocationIdAsTypeAsync(Guid colocationId)
        {
            return await context.Users
                .Where(u => u.ColocationId == colocationId)
                .Select(u => new UserOutput
                {
                    Id = u.Id,
                    ColocationId = u.ColocationId,
                    Email = u.Email,
                    Username = u.Username,
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<BalanceOutput>> GetAllBalancesOutputFromColocationIdAsync(Guid colocationId)
        {
            return await context.Balances
                .Where(b => b.User.ColocationId == colocationId)
                .Select(b => new BalanceOutput
                {
                    UserId = b.UserId,
                    PersonalBalance = b.PersonalBalance,
                    LastUpdate = b.LastUpdate,
                })
                .AsNoTracking()
                .OrderBy(x => x.PersonalBalance)
                .ToListAsync();
        }

        public async Task AddChoreEnrollmentAsync(ChoreEnrollment choreEnrollment)
        {
            await context.ChoreEnrollments.AddAsync(choreEnrollment);
        }

        public async Task<int> AddSplitBetweenRangeAsync(List<SplitBetween> splitList)
        {
            await context.SplitBetweens.AddRangeAsync(splitList);
            return splitList.Count;
        }

        public int UpdateBalanceRange(List<Balance> balances)
        {
            
            context.Balances.UpdateRange(balances);
            return balances.Count;
        }

        public async Task AddBalanceAsync(Balance balance)
        {
            await context.Balances.AddAsync(balance);
        }

        public async Task<int> DeleteRangeSplitBetweenExpenseId(Guid expenseId)
        {
            var splitbetweens = await context.SplitBetweens.Where(x => x.ExpenseId == expenseId).ToListAsync();
            context.SplitBetweens.RemoveRange(splitbetweens);
            return splitbetweens.Count;
        }

        public async Task<int> DeleteRangeEntriesByExpenseId(Guid expenseId)
        {
            var entries = await context.Entries.Where(x => x.ExpenseId == expenseId).ToListAsync();
            context.Entries.RemoveRange(entries);
            return entries.Count();
        }

    }
}

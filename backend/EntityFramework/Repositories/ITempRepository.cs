using EntityFramework.Models;
using Shared.Models.Output;

namespace EntityFramework.Repositories
{
    public interface ITempRepository
    {
        Task<List<ChoreMessageOutput>> GetAllChoreMessageOutputByChoreIdAsync(Guid choreId);
        Task<int> DeleteRangeChoreMessageFromChoreId(Guid choreId);
        Task<List<UserOutput>> GetEnrolledUserOutputFromChoreIdAsync(Guid choreId);
        Task<List<ChoreOutput>> GetEnrolledChoreOutputFromUserIdAsync(Guid userId);
        Task RemoveChoreEnrollmentAsync(Guid userId, Guid choreId);
        Task<List<Balance>> GetBalanceFromUserIdListAsync(List<Guid> userList);
        Task DeleteAllEntriesByExpenseId(Guid expenseId);
        Task<List<BalanceOutput>> GetAllByColocationIdAsync(Guid colocationId);
        Task<List<Balance>> GetAllBalancesFromColocationIdListAsync(Guid colocationId);
        Task<bool> AnyExistingUserByEmail(string email);
        Task<User?> GetUserByEmailAsync(string email);
        Task<List<UserOutput>> GetAllByColocationIdAsTypeAsync(Guid colocationId);
        Task<List<BalanceOutput>> GetAllBalancesOutputFromColocationIdAsync(Guid colocationId);
    }
}

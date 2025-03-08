using EntityFramework.Models;
using Shared.Models.Output;

namespace EntityFramework.Repositories.Interfaces
{
    public interface IChoreRepository
    {
        Task<List<ChoreOutput>> GetAllChoreOutputsAsync(Guid CollocationId);
        Task<ChoreOutput?> GetChoreOutputByIdAsync(Guid id);
        Task<List<ChoreMessageOutput>> GetAllChoreMessageOutputByChoreIdAsync(Guid choreId);
        Task AddChoreAsync(Chore chore);
        Task AddChoreMessageAsync(ChoreMessage choreMessage);
        Task<Chore?> GetChoreByIdAsync(Guid id);
        Task UpdateChoreAsync(Chore chore);
        Task DeleteChoreAsync(Chore chore);
        Task<List<ChoreMessage>?> GetChoreMessageFromChoreId(Guid choreId);
        Task DeleteRangeChoreMessageFromChoreId(List<ChoreMessage> choreMessages);
        Task<List<UserOutput>> GetEnrolledUserOutputFromChoreIdAsync(Guid choreId);
        Task<List<ChoreOutput>> GetEnrolledChoreOutputFromUserIdAsync(Guid userId);
        Task AddChoreEnrollmentAsync(ChoreEnrollment choreEnrollment);
        Task<ChoreEnrollment?> GetChoreEnrollmentByUserIdAndChoreIdAsync(Guid userId, Guid choreId);
        Task RemoveChoreEnrollmentAsync(ChoreEnrollment choreEnrollment);
        Task SaveChangesAsync();
    }
}

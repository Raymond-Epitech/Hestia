using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Interfaces
{
    public interface IChoreService
    {
        Task<List<ChoreOutput>> GetAllChoresAsync(Guid CollocationId);
        Task<ChoreOutput> GetChoreAsync(Guid id);
        Task<List<ChoreMessageOutput>> GetChoreMessageFromChoreAsync(Guid id);
        Task<Guid> AddChoreAsync(ChoreInput input);
        Task<Guid> AddChoreMessageAsync(ChoreMessageInput input);
        Task UpdateChoreAsync(ChoreUpdate input);
        Task DeleteChoreAsync(Guid id);
        Task DeleteChoreMessageByChoreIdAsync(Guid id);
        Task<List<ChoreOutput>> GetChoreFromUser(Guid UserId);
        Task<List<UserOutput>> GetUserFromChore(Guid ChoreId);
        Task EnrollToChore(Guid UserId, Guid ChoreId);
        Task UnenrollToChore(Guid UserId, Guid ChoreId);
    }
}

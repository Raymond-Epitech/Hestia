using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Interfaces
{
    public interface IChoreService
    {
        Task<List<ChoreOutput>> GetAllChoresAsync(Guid CollocationId);
        Task<ChoreOutput> GetChoreAsync(Guid id);
        Task<Guid> AddChoreAsync(ChoreInput input);
        Task<Guid> UpdateChoreAsync(ChoreUpdate input);
        Task<Guid> MarkChoreAsDoneAsync(Guid id);
        Task<Guid> DeleteChoreAsync(Guid id);
        Task<List<ChoreOutput>> GetChoreFromUser(Guid UserId);
        Task<List<UserOutput>> GetUserFromChore(Guid ChoreId);
        Task<Guid> EnrollToChore(Guid UserId, Guid ChoreId);
        Task<Guid> UnenrollToChore(Guid UserId, Guid ChoreId);
    }
}

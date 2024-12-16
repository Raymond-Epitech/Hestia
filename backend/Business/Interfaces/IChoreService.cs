using Business.Models.Input;
using Business.Models.Output;
using Business.Models.Update;

namespace Business.Interfaces
{
    public interface IChoreService
    {
        Task<List<ChoreOutput>> GetAllChoresAsync(Guid CollocationId);
        Task<ChoreOutput> GetChoreAsync(Guid id);
        Task<List<ChoreMessageOutput>> GetChoreMessageFromChoreAsync(Guid id);
        Task AddChoreAsync(ChoreInput input);
        Task AddChoreMessageAsync(ChoreMessageInput input);
        Task UpdateChoreAsync(ChoreUpdate input);
        Task DeleteChoreAsync(Guid id);
        Task DeleteChoreMessageByChoreIdAsync(Guid id);
    }
}

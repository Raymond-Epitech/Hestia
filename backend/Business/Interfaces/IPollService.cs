using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Interfaces;

public interface IPollService
{
    Task<List<PollVoteOutput>> GetAllPollVoteAsync(Guid pollReminderId);
    Task<Guid> AddPollVoteAsync(PollVoteInput vote);
    Task<Guid> UpdatePollVoteAsync(PollVoteUpdate vote);
    Task<Guid> DeletePollVoteAsync(Guid id);

}


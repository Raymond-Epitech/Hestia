using Shared.Models.Input;
using Shared.Models.Output;

namespace Business.Interfaces;

public interface IReactionService
{
    Task<List<ReactionOutput>> GetReactionsByPostIdAsync(Guid ReminderId);
    Task<Guid> AddReactionAsync(ReactionInput reactionInput);
    Task<Guid> DeleteReactionAsync(ReactionInputForDelete input);
}


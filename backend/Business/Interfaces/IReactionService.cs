using Shared.Models.Input;

namespace Business.Interfaces;

public interface IReactionService
{
    Task<List<string>> GetReactionsByPostIdAsync(Guid ReminderId);
    Task<Guid> AddReactionAsync(ReactionInput reactionInput);
    Task<Guid> DeleteReactionAsync(ReactionInputForDelete input);
}


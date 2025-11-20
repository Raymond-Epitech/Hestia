using Shared.Models.Input;

namespace Business.Interfaces;

public interface IFeedbackService
{
    Task<List<FeedbackOutput>> GetAllFeedbacks();
    Task<Guid> AddFeedback(FeedbackInput input);
}


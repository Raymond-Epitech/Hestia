using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Interfaces
{
    public interface IReminderService
    {
        Task<List<ReminderOutput>> GetAllRemindersAsync(Guid ColocationId);
        Task<ReminderOutput> GetReminderAsync(Guid id);
        Task<Guid> AddReminderAsync(ReminderInput input);
        Task UpdateReminderAsync(ReminderUpdate input);
        Task UpdateRangeReminderAsync(List<ReminderUpdate> inputs);
        Task DeleteReminderAsync(Guid id);
    }
}

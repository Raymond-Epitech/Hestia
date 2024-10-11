using Business.Models.Input;
using Business.Models.Output;

namespace Business.Interfaces
{
    public interface IReminderService
    {
        Task<List<ReminderOutput>> GetAllRemindersAsync();
        Task<ReminderOutput> GetReminderAsync(Guid id);
        Task AddReminderAsync(ReminderInput input);
        Task UpdateReminderAsync(ReminderUpdate input);
        Task UpdateRangeReminderAsync(List<ReminderUpdate> inputs);
        Task DeleteReminderAsync(Guid id);
    }
}

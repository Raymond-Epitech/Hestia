using Business.Models.Input;
using Business.Models.Output;
using Business.Models.Update;

namespace Business.Interfaces
{
    public interface IReminderService
    {
        Task<List<ReminderOutput>> GetAllRemindersAsync(Guid CollocationId);
        Task<ReminderOutput> GetReminderAsync(Guid id);
        Task AddReminderAsync(ReminderInput input);
        Task UpdateReminderAsync(ReminderUpdate input);
        Task UpdateRangeReminderAsync(List<ReminderUpdate> inputs);
        Task DeleteReminderAsync(Guid id);
    }
}

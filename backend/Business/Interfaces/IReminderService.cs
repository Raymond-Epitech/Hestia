using Business.Models.Input;
using Business.Models.Output;

namespace Business.Interfaces
{
    public interface IReminderService
    {
        Task<List<ReminderOutput>> GetAllRemindersAsync();
        Task<ReminderOutput> GetReminderAsync(Guid id);
        Task AddReminderAsync(ReminderInput input);
        Task UpdateReminderAsync(Guid id, ReminderInput input);
        Task DeleteReminderAsync(Guid id);
    }
}

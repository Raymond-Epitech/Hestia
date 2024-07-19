using Business.Models.Input;
using Business.Models.Output;

namespace Business.Interfaces
{
    public interface IReminderService
    {
        Task<ReminderOutput> GetReminderAsync(Guid id);
        Task AddReminderAsync(ReminderInput input);
        Task UpdateReminderAsync(ReminderInput input);
        Task DeleteReminderAsync(Guid id);
    }
}

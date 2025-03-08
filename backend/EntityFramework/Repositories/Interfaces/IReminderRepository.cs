using EntityFramework.Models;
using Shared.Models.Output;

namespace EntityFramework.Repositories.Interfaces
{
    public interface IReminderRepository
    {
        Task<List<ReminderOutput>> GetAllReminderOutputsAsync(Guid colocationId);
        Task<ReminderOutput?> GetReminderOutputAsync(Guid id);
        Task<Reminder?> GetReminderAsync(Guid id);
        Task AddReminderAsync(Reminder reminder);
        Task UpdateReminderAsync(Reminder reminder);
        Task DeleteReminderAsync(Reminder reminder);
        Task SaveChangesAsync();
    }
}

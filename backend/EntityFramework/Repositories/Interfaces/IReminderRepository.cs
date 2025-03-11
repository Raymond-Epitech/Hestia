using EntityFramework.Models;
using Microsoft.EntityFrameworkCore.Storage;
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
        Task<List<Reminder>> GetReminderFromListOfId(List<Guid> ids);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task SaveChangesAsync();
    }
}

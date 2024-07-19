using Business.Models.Output;

namespace Business.Interfaces
{
    public interface IReminderService
    {
        Task<ReminderOutput> GetReminderAsync(Guid id);
    }
}

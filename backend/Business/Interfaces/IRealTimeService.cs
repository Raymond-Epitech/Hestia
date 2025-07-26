using Shared.Models.Output;

namespace Business.Interfaces;

public interface IRealTimeService
{
    Task NotifyReminderAdded(ReminderOutput reminder);
    Task NotifyReminderUpdated(ReminderOutput reminder);
    Task NotifyReminderUpdateRange(List<ReminderOutput> reminders);
    Task NotifyReminderDeleted(Guid reminderId);
}


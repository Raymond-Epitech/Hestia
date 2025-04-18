﻿using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Interfaces
{
    public interface IReminderService
    {
        Task<List<ReminderOutput>> GetAllRemindersAsync(Guid ColocationId);
        Task<ReminderOutput> GetReminderAsync(Guid id);
        Task<Guid> AddReminderAsync(ReminderInput input);
        Task<Guid> UpdateReminderAsync(ReminderUpdate input);
        Task<int> UpdateRangeReminderAsync(List<ReminderUpdate> inputs);
        Task<Guid> DeleteReminderAsync(Guid id);
    }
}

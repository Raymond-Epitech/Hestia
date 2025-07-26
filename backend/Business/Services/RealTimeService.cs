using Business.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Shared.Models.Output;
using SignalRChat.Hubs;

namespace Business.Services;

public class RealTimeService : IRealTimeService
{
    private readonly IHubContext<HestiaHub> _hubContext;

    public RealTimeService(IHubContext<HestiaHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyReminderAdded(ReminderOutput reminder)
    {
        await _hubContext.Clients.All.SendAsync("NewReminderAdded", reminder);
    }

    public async Task NotifyReminderUpdated(ReminderOutput reminder)
    {
        await _hubContext.Clients.All.SendAsync("ReminderUpdated", reminder);
    }

    public async Task NotifyReminderUpdateRange(List<ReminderOutput> reminders)
    {
        await _hubContext.Clients.All.SendAsync("ReminderUpdateRange", reminders);
    }

    public async Task NotifyReminderDeleted(Guid reminderId)
    {
        await _hubContext.Clients.All.SendAsync("ReminderDeleted", reminderId);
    }
}


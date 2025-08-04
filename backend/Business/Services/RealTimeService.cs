using Business.Interfaces;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;

namespace Business.Services;

public class RealTimeService : IRealTimeService
{
    private readonly IHubContext<HestiaHub> _hubContext;

    public RealTimeService(IHubContext<HestiaHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendToGroupAsync(Guid colocationId, string methodName, object? payload)
    {
        var groupName = $"colocation:{colocationId}";
        await _hubContext.Clients.Group(groupName).SendAsync(methodName, payload);
    }
}


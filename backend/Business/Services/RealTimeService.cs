using Business.Interfaces;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;

namespace Business.Services;

public class RealTimeService(IHubContext<HestiaHub> hubContext) : IRealTimeService
{
    public async Task SendToGroupAsync(Guid colocationId, string methodName, object? payload)
    {
        var groupName = $"colocation:{colocationId}";
        await hubContext.Clients.Group(groupName).SendAsync(methodName, payload);
    }
}


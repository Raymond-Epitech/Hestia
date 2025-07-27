using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs;

public class HestiaHub : Hub
{
    public async Task JoinColocationGroup(Guid colocationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"colocation:{colocationId}");
    }
}

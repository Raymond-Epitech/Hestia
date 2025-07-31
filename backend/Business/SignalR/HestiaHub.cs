using Microsoft.AspNetCore.SignalR;

namespace SignalR.Hubs;

public class HestiaHub : Hub
{
    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"Client connected: {Context.ConnectionId}");
        return base.OnConnectedAsync();
    }
    public async Task JoinColocationGroup(Guid colocationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"colocation:{colocationId}");
    }
}

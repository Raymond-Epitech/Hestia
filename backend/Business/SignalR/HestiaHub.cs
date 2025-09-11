using Microsoft.AspNetCore.SignalR;

namespace SignalR.Hubs;

public class HestiaHub : Hub
{
    public static int ConnectionCount = 0;

    public override Task OnConnectedAsync()
    {
        Interlocked.Increment(ref ConnectionCount);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? ex)
    {
        Interlocked.Decrement(ref ConnectionCount);
        return base.OnDisconnectedAsync(ex);
    }

    public Task Ping() => Task.CompletedTask;

    public async Task JoinColocationGroup(Guid colocationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"colocation:{colocationId}");
    }
}

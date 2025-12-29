using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace SignalR.Hubs;

public class HestiaHub(ILogger<HestiaHub> logger) : Hub
{
    public override async Task OnConnectedAsync()
    {
        logger.LogInformation("SignalR: Client connecté {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception != null)
            logger.LogWarning("SignalR: Déconnexion anormale {ConnectionId}: {Message}", Context.ConnectionId, exception.Message);
        else
            logger.LogInformation("SignalR: Client déconnecté {ConnectionId}", Context.ConnectionId);

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinColocationGroup(Guid colocationId)
    {
        var groupName = $"colocation:{colocationId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        logger.LogInformation("SignalR: Client {ConnectionId} a rejoint {GroupName}", Context.ConnectionId, groupName);
    }
}

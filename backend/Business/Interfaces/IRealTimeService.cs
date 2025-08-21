using Shared.Models.Output;

namespace Business.Interfaces;

public interface IRealTimeService
{
    Task SendToGroupAsync(Guid colocationId, string methodName, object? payload);
}


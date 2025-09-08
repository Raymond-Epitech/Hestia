using Shared.Models.Input;

namespace Business.Interfaces;

public interface IFirebaseNotificationService
{
    Task<Guid> SendNotificationToUserAsync(NotificationInput notification);
    Task<List<Guid>> SendNotificationToColocationAsync(NotificationInput notification, Guid? UserId);
}


namespace Business.Interfaces;

public interface IFirebaseNotificationService
{
    Task SendNotificationAsync(List<string> fcmDevices, string title, string body);
}


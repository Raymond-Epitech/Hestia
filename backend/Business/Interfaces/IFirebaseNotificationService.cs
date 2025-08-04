namespace Business.Interfaces;

public interface IFirebaseNotificationService
{
    Task SendNotificationAsync(string title, string body);
}


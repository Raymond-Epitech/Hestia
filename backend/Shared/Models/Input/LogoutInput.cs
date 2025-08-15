namespace Shared.Models.Input;

public class LogoutInput
{
    public Guid UserId { get; set; }
    public string FCMToken { get; set; } = null!;
}


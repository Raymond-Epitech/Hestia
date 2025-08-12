namespace Shared.Models.Configuration;

public class FirebaseSettings
{
    public string ServerKey { get; set; } = null!;
    public string Url { get; set; } = null!;
    public List<string> Tokens { get; set; } = new();
}

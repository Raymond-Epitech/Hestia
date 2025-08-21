namespace Shared.Models.DTO;

public class FileDTO
{
    public byte[] Content { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
}


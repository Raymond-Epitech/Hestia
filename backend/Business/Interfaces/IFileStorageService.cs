using Shared.Models.DTO;

namespace Business.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string folder);
    Task<Stream> GetAsync(string folder, string fileName);
    Task DeleteAsync(string folder, string fileName);
}

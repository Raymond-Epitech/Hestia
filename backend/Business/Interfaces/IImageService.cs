using Microsoft.AspNetCore.Http;
using Shared.Models.DTO;

namespace Business.Interfaces;

public interface IImageService
{
    string GetContentType(string path);
    Task<FileDTO> GetImageByNameAsync(string fileName);
    Task<byte[]> CompressImageAsync(Stream imageStream, int quality = 50);
    Task<string> SaveImage(IFormFile file);
    string DeleteImage(string fileName);
    Task<string> DownloadImageAsFormFileAsync(string imageUrl);
}


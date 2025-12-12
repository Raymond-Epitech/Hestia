using Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Shared.Exceptions;
using Shared.Models.DTO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Business.Services;

public class ImageService : IImageService
{
    private string ImageRoute => "wwwroot/uploads/";

    /// <summary>
    /// Get the content type of a file based on its extension
    /// </summary>
    /// <param name="path">The path of the file to get the content type</param>
    /// <returns>The correct content path</returns>
    public string GetContentType(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".pdf" => "application/pdf",
            ".txt" => "text/plain",
            _ => "application/octet-stream"
        };
    }

    /// <summary>
    /// Get an image by its name
    /// </summary>
    /// <param name="fileName">The name of the file</param>
    /// <returns>The file's byte and its parameters</returns>
    /// <exception cref="InvalidDataException"></exception>
    /// <exception cref="NotFoundException"></exception>
    public async Task<FileDTO> GetImageByNameAsync(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new InvalidDataException("File name invalid");

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), ImageRoute, fileName);

        if (!File.Exists(filePath))
            throw new NotFoundException("Image not found");

        var file = new FileDTO
        {
            FileName = fileName,
            ContentType = GetContentType(filePath),
            Content = await File.ReadAllBytesAsync(filePath)
        };

        if (file.Content == null)
        {
            throw new NotFoundException("Image not found");
        }

        return file;
    }

    /// <summary>
    /// Compress an image to jpeg format
    /// </summary>
    /// <param name="imageStream">The byte stream of the image</param>
    /// <param name="quality">The quality (default is 75%)</param>
    /// <returns>A new byte stream</returns>
    public async Task<byte[]> CompressImageAsync(Stream imageStream, int quality = 50)
    {
        using var image = await Image.LoadAsync(imageStream);

        using var outputStream = new MemoryStream();

        var encoder = new JpegEncoder
        {
            Quality = quality
        };

        await image.SaveAsync(outputStream, encoder);

        return outputStream.ToArray();
    }

    /// <summary>
    /// Save an image to the server
    /// </summary>
    /// <param name="file">The file</param>
    /// <returns>the new name of the file</returns>
    public async Task<string> SaveImage(IFormFile file)
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), ImageRoute);

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        byte[] compressedImage;
        using (var inputStream = file.OpenReadStream())
        {
            compressedImage = await CompressImageAsync(inputStream, quality: 75);
        }

        await File.WriteAllBytesAsync(filePath, compressedImage);

        return uniqueFileName;
    }

    /// <summary>
    /// Delete an image from the server
    /// </summary>
    /// <param name="fileName">The file name</param>
    /// <returns>The file name</returns>
    /// <exception cref="InvalidDataException"></exception>
    public string DeleteImage(string fileName)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), ImageRoute, fileName);

        if (!File.Exists(filePath))
            throw new NotFoundException("Image not found");

        File.Delete(filePath);

        return fileName;
    }

    public async Task<string> DownloadImageAsFormFileAsync(string imageUrl)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(imageUrl);

        response.EnsureSuccessStatusCode();

        var stream = new MemoryStream();
        await response.Content.CopyToAsync(stream);
        stream.Position = 0;

        var formFile = new FormFile(stream, 0, stream.Length, "file", "tmp.jpeg")
        {
            Headers = new HeaderDictionary(),
            ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream"
        };

        return await SaveImage(formFile);
    }
}


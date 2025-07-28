using Business.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Business.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _rootPath;

    public LocalFileStorageService(IConfiguration config)
    {
        _rootPath = config["Storage:RootPath"] ?? "wwwroot/uploads";
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string folder)
    {
        var folderPath = Path.Combine(_rootPath, folder);
        Directory.CreateDirectory(folderPath);

        var fullPath = Path.Combine(folderPath, fileName);

        using var output = File.Create(fullPath);
        await fileStream.CopyToAsync(output);

        return fileName;
    }

    public Task<Stream> GetAsync(string folder, string fileName)
    {
        var file = FindFileByName(folder, fileName);
        if (file is null)
            throw new FileNotFoundException($"File {fileName} not found in folder {folder}");

        var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
        return Task.FromResult<Stream>(stream);
    }

    public Task DeleteAsync(string folder, string fileName)
    {
        var file = FindFileByName(folder, fileName);
        if (file != null && File.Exists(file.FullName))
            File.Delete(file.FullName);

        return Task.CompletedTask;
    }

    private FileInfo? FindFileByName(string folder, string baseFileName)
    {
        var directory = new DirectoryInfo(Path.Combine(_rootPath, folder));
        return directory.Exists
            ? directory.GetFiles($"{baseFileName}.*").FirstOrDefault()
            : null;
    }
}

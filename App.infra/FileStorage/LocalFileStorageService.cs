using App.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace App.infra.FileStorage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _storagePath;
    private readonly string _baseUrl;

    public LocalFileStorageService(IConfiguration configuration)
    {
        _storagePath = configuration["FileStorage:Path"] ?? "Storage";
        _baseUrl = configuration["FileStorage:BaseUrl"] ?? "/storage/";
        
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType)
    {
        var extension = Path.GetExtension(fileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(_storagePath, uniqueFileName);
        
        using var outputStream = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(outputStream);
        
        return Path.Combine(_baseUrl, uniqueFileName).Replace("\\", "/");
    }

    public async Task DeleteFileAsync(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl))
            return;

        var fileName = Path.GetFileName(fileUrl);
        var filePath = Path.Combine(_storagePath, fileName);
        
        if (File.Exists(filePath))
        {
            await Task.Run(() => File.Delete(filePath));
        }
    }

    public Task<Stream?> GetFileStreamAsync(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl))
            return Task.FromResult<Stream?>(null);

        var fileName = Path.GetFileName(fileUrl);
        var filePath = Path.Combine(_storagePath, fileName);
        
        if (!File.Exists(filePath))
            return Task.FromResult<Stream?>(null);

        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return Task.FromResult<Stream?>(stream);
    }
}

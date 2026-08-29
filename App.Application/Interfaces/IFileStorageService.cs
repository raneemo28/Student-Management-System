namespace App.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType);
    Task DeleteFileAsync(string fileUrl);
    Task<Stream?> GetFileStreamAsync(string fileUrl);
}

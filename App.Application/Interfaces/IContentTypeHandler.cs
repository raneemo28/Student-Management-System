namespace App.Application.Interfaces;

public interface IContentTypeHandler
{
    bool CanHandle(string contentType);
    Task<string> GetFilePathAsync(string fileUrl);
}

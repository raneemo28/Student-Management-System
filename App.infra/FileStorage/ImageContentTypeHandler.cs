using App.Application.Interfaces;

namespace App.infra.FileStorage;

public class ImageContentTypeHandler : IContentTypeHandler
{
    public bool CanHandle(string contentType)
    {
        return contentType.Equals("Image", StringComparison.OrdinalIgnoreCase);
    }

    public Task<string> GetFilePathAsync(string fileUrl)
    {
        return Task.FromResult(fileUrl);
    }
}

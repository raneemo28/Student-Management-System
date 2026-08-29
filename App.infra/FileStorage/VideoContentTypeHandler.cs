using App.Application.Interfaces;

namespace App.infra.FileStorage;

public class VideoContentTypeHandler : IContentTypeHandler
{
    public bool CanHandle(string contentType)
    {
        return contentType.Equals("Video", StringComparison.OrdinalIgnoreCase);
    }

    public Task<string> GetFilePathAsync(string fileUrl)
    {
        return Task.FromResult(fileUrl);
    }
}

using App.Application.Interfaces;

namespace App.infra.FileStorage;

public class PdfContentTypeHandler : IContentTypeHandler
{
    public bool CanHandle(string contentType)
    {
        return contentType.Equals("Pdf", StringComparison.OrdinalIgnoreCase);
    }

    public Task<string> GetFilePathAsync(string fileUrl)
    {
        return Task.FromResult(fileUrl);
    }
}

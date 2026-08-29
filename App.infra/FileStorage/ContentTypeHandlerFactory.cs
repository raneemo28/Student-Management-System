using App.Application.Interfaces;

namespace App.infra.FileStorage;

public class ContentTypeHandlerFactory
{
    private readonly IEnumerable<IContentTypeHandler> _handlers;

    public ContentTypeHandlerFactory(IEnumerable<IContentTypeHandler> handlers)
    {
        _handlers = handlers;
    }

    public IContentTypeHandler? GetHandler(string contentType)
    {
        return _handlers.FirstOrDefault(h => h.CanHandle(contentType));
    }
}

using System.Text.Json;

namespace App.Application.Common.Events;

public static class DomainEventSerializer
{
    public static string Serialize(object data)
    {
        return JsonSerializer.Serialize(data);
    }

    public static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json);
    }
}

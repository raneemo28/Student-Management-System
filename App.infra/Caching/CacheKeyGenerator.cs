namespace App.infra.Caching;

public static class CacheKeyGenerator
{
    public static string ForEntityById(string entityType, string id)
        => $"read:{entityType}:{id}";

    public static string ForAll(string entityType)
        => $"read:{entityType}:all";

    public static string ForQuery(string queryName, string? parameter = null)
        => parameter != null
            ? $"read:{queryName}:{parameter}"
            : $"read:{queryName}";

    public static string ForEntityListByForeignKey(string entityType, string foreignKey, string foreignId)
        => $"read:{entityType}:{foreignKey}:{foreignId}";
}

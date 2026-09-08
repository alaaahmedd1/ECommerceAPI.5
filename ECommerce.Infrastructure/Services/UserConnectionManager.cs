using System.Collections.Concurrent;

namespace ECommerce.Infrastructure.Services;

public static class UserConnectionManager
{
    private static readonly ConcurrentDictionary<string, string> _connections = new();

    public static void AddConnection(string userId, string connectionId)
    {
        _connections[userId] = connectionId;
    }

    public static void RemoveConnection(string userId)
    {
        _connections.TryRemove(userId, out _);
    }

    public static string? GetConnection(string userId)
    {
        _connections.TryGetValue(userId, out var connectionId);
        return connectionId;
    }
}
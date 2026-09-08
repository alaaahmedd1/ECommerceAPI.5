using ECommerce.Application.Interfaces.Services;
using StackExchange.Redis;
using System.Text.Json;

namespace ECommerce.Infrastructure.Services;

public sealed class CacheService<T> : ICacheService<T> where T : class
{
    private readonly IDatabase _database;

    public CacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    public async Task<T?> GetCacheAsync(string key, CancellationToken cancellationToken = default)
    {
        var cachedValue = await _database.StringGetAsync(key);

        if (cachedValue.IsNullOrEmpty)
        {
            return null;
        }

        return JsonSerializer.Deserialize<T>(cachedValue!, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    public async Task SetCacheAsync(string key, T value, CancellationToken cancellationToken = default)
    {
        var jsonValue = JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await _database.StringSetAsync(key, jsonValue);
    }

    public async Task RemoveCacheAsync(string key, CancellationToken cancellationToken = default)
    {
        await _database.KeyDeleteAsync(key);
    }
}
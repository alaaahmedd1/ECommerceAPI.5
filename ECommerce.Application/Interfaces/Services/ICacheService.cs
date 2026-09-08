namespace ECommerce.Application.Interfaces.Services;

public interface ICacheService<T> where T : class
{
    Task<T?> GetCacheAsync(string key, CancellationToken cancellationToken = default);
    Task SetCacheAsync(string key, T value, CancellationToken cancellationToken = default);
    Task RemoveCacheAsync(string key, CancellationToken cancellationToken = default);
}
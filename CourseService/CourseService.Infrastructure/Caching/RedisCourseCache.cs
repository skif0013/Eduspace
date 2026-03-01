using CourseService.Application.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace CourseService.Infrastructure.Caching;

public class RedisCourseCache : ICourseCache
{
    private const string CatalogVersionKey = "catalog:version";

    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _multiplexer;
    private readonly RadisCacheSettings _settings;

    public RedisCourseCache(
        IDistributedCache cache,
        IConnectionMultiplexer multiplexer,
        IOptions<RadisCacheSettings> options)
    {
        _cache = cache;
        _multiplexer = multiplexer;
        _settings = options.Value;
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        try
        {
            var data = await _cache.GetStringAsync(key);
            if (string.IsNullOrWhiteSpace(data))
            {
                return null;
            }

            return JsonSerializer.Deserialize<T>(data);
        }
        catch (RedisConnectionException)
        {
            return null;
        }
    }


    public async Task SetAsync<T>(string key, T value, CacheEntryType type) where T : class
    {
        var expiration = type switch
        {
            CacheEntryType.Catalog =>
                TimeSpan.FromMinutes(_settings.CatalogExpirationMinutes),

            CacheEntryType.Course =>
                TimeSpan.FromMinutes(_settings.CourseExpirationMinutes),

            _ => TimeSpan.FromMinutes(5)
        };

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };

        var json = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, json, options);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }

    public async Task<long> GetCatalogVersionAsync()
    {
        try
        {
            var version = await _cache.GetStringAsync(CatalogVersionKey);

            if (version == null)
            {
                await _cache.SetStringAsync(CatalogVersionKey, "1");
                return 1;
            }

            return long.Parse(version);
        }
        catch (RedisConnectionException)
        {
            return 1;
        }
    }

    public async Task IncrementCatalogVersionAsync()
    {
        var db = _multiplexer.GetDatabase();
        await db.StringIncrementAsync(CatalogVersionKey);
    }
}

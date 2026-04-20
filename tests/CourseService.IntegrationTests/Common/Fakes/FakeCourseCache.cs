using CourseService.Application.Caching;

namespace CourseService.IntegrationTests.Common.Fakes;

public class FakeCourseCache : ICourseCache
{
    private long _catalogVersion = 1;

    public Task<T?> GetAsync<T>(string key) where T : class
    {
        return Task.FromResult<T?>(null);
    }

    public Task<long> GetCatalogVersionAsync()
    {
        return Task.FromResult(_catalogVersion);
    }

    public Task IncrementCatalogVersionAsync()
    {
        _catalogVersion++;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        return Task.CompletedTask;
    }

    public Task SetAsync<T>(string key, T value, CacheEntryType type) where T : class
    {
        return Task.CompletedTask;
    }
}

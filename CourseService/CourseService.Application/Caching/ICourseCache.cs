namespace CourseService.Application.Caching;

public interface ICourseCache
{
    Task<T?> GetAsync<T>(string key) where T : class;
    Task SetAsync<T>(string key, T value, CacheEntryType type) where T : class;
    Task RemoveAsync(string key);

    Task<int> GetCatalogVersionAsync();
    Task IncrementCatalogVersionAsync();
}

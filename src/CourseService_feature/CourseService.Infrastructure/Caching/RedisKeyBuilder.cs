using CourseService.Application.Caching;
using Microsoft.Extensions.Options;

namespace CourseService.Infrastructure.Caching;

public class RedisKeyBuilder : IRedisKeyBuilder
{
    private readonly string _prefix;

    public RedisKeyBuilder(IOptions<RadisCacheSettings> options)
    {
        _prefix = options.Value.KeyPrefix;
    }

    public string GetCourseKey(Guid courseId)
    {
        return $"{_prefix}:course:{courseId}";
    }

    public string GetCoursesPageKey(long version, int page, int size)
    {
        return $"{_prefix}:courses:v{version}:page:{page}:size:{size}";
    }

    public string GetCatalogVersion()
    {
        return $"{_prefix}:catalog:version";
    }
}

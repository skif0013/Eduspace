using CourseService.Application.Caching;

namespace CourseService.IntegrationTests.Common.Fakes;

public class FakeRedisKeyBuilder : IRedisKeyBuilder
{
    public string GetCatalogVersion()
    {
        return "test-catalog-version";
    }

    public string GetCourseKey(Guid courseId)
    {
        return $"test-course-{courseId}";
    }

    public string GetCoursesPageKey(long version, int page, int size)
    {
        return $"test-courses-{version}-{page}-{size}";
    }
}

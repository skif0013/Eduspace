namespace CourseService.Application.Caching;

public interface IRedisKeyBuilder
{
    string GetCourseKey(Guid courseId);
    string GetCoursesPageKey(long version, int page, int size);
    string GetCatalogVersion();
}

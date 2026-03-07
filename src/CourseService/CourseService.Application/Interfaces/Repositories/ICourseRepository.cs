using CourseService.Domain.Entities;

namespace CourseService.Application.Interfaces.Repositories;

public interface ICourseRepository
{
    Task<Course> CreateCourseAsync(Course course);
    Task<Course> GetCourseByIdAsync(Guid courseId);
    Task<List<Course>> GetAllCoursesAsync();
    Task UpdateCourseAsync(Course course);
}

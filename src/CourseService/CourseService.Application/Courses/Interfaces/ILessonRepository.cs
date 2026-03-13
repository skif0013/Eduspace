using CourseService.Domain.Entities;

namespace CourseService.Application.Courses.Interfaces;

public interface ILessonRepository
{
    Task<Lesson> CreateLessonAsync(Lesson lesson);
    Task<Lesson> GetLessonByIdAsync(Guid lessonId);
    Task<List<Lesson>> GetLessonsAsync(Guid courseId);
    Task UpdateLessonAsync(Lesson lesson);
}

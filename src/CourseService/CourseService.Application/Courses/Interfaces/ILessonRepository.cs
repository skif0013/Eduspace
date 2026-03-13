using CourseService.Domain.Entities;

namespace CourseService.Application.Courses.Interfaces;

public interface ILessonRepository
{
    Task<Lesson> CreateLessonAsync(Lesson lesson);
    Task<Lesson> GetLessonByIdAsync(Guid lessonId);
    Task<Lesson> GetLessonsAsync();
    Task UpdateLessonAsync(Lesson lesson);
}

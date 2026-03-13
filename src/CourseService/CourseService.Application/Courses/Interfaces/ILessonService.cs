using CourseService.Application.Courses.DTO;
using CourseService.Domain.Abstractions;

namespace CourseService.Application.Courses.Interfaces;

public interface ILessonService
{
    Task<Result<LessonResponse>> CreateLessonAsync(LessonDTO lessonDTO, Guid courseId, Guid authorId);
    Task<Result> DeleteLessonAsync(Guid lessonId, Guid courseId, Guid authorId);
    Task<Result<LessonResponse>> GetLessonByIdAsync(Guid lessonId);
    Task<Result<LessonResponse>> UpdateLessonAsync(LessonDTO lessonDTO, Guid lessonId, Guid courseId, Guid authorId);
}

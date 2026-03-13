using CourseService.Application.Courses.DTO;
using CourseService.Domain.Abstractions;

namespace CourseService.Application.Courses.Interfaces;

public interface ILessonService
{
    Task<Result> ArchiveLessonAsync(Guid lessonId, Guid courseId, Guid authorId);
    Task<Result<CourseResponse>> CreateLessonAsync(LessonDTO lessonDTO, Guid courseId, Guid authorId);
    Task<Result<CourseResponse>> GetLessonByIdAsync(Guid lessonId);
    Task<Result> PublishLessonAsync(Guid lessonId, Guid courseId, Guid authorId);
    Task<Result<CourseResponse>> UpdateLessonAsync(LessonDTO lessonDTO, Guid lessonId, Guid courseId, Guid authorId);
}

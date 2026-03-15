using CourseService.Application.Courses.DTO;
using CourseService.Domain.Abstractions;

namespace CourseService.Application.Courses.Interfaces;

public interface ICourseRatingService
{
    Task<Result<CourseRatingResponse>> CreateRatingAsync(CourseRatingDTO ratingDTO, Guid courseId, Guid userId);
    Task<Result<CourseRatingResponse>> UpdateRatingAsync(CourseRatingDTO ratingDTO, Guid courseId, Guid userId);
}

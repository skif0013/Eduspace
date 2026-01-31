using CourseService.Application.DTO;
using CourseService.Domain.Results;

namespace CourseService.Application.Interfaces.Services;

public interface ICourseRatingService
{
    Task<Result<CourseRatingResponse>> CreateRatingAsync(CourseRatingDTO ratingDTO, Guid courseId, Guid userId);
    Task<Result<CourseRatingResponse>> UpdateRatingAsync(CourseRatingDTO ratingDTO, Guid courseId, Guid userId);
}

using CourseService.Application.DTO;
using CourseService.Application.Interfaces.Repositories;
using CourseService.Application.Interfaces.Services;
using CourseService.Domain.Entities;
using CourseService.Domain.Results;

namespace CourseService.Application.Services;

public class CourseRatingService : ICourseRatingService
{
    private readonly ICourseRatingRepository _ratingRepository;
    private readonly ICourseRepository _courseRepository;

    public CourseRatingService(
        ICourseRatingRepository ratingRepository,
        ICourseRepository courseRepository)
    {
        _ratingRepository = ratingRepository;
        _courseRepository = courseRepository;
    }

    public async Task<Result<bool>> CreateRatingAsync(CourseRatingDTO ratingDTO, Guid courseId, Guid userId)
    {
        var courseExists = await _courseRepository.CourseExistAsync(courseId);
        if (!courseExists)
        {
            return Result<bool>.Failure($"Course with {courseId} not found");
        }

        var ratingExists = await _ratingRepository.GetRatingByCourseIdAdnUserIdAsync(courseId, userId);   
        if(ratingExists != null)
        {
            return Result<bool>.Failure($"User with {userId} has already rated this course.");
        }

        var rating = new CourseRating
        {
            CourseId = courseId,
            UserId = userId,
            Rating = ratingDTO.Rating,
        };

        await _ratingRepository.CreateRatingAsync(rating);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> UpdateRatingAsync(CourseRatingDTO ratingDTO, Guid courseId, Guid userId)
    {
        var courseExists = await _courseRepository.CourseExistAsync(courseId);
        if (!courseExists)
        {
            return Result<bool>.Failure($"Course with {courseId} not found");
        }

        var existingRating = await _ratingRepository.GetRatingByCourseIdAdnUserIdAsync(courseId, userId);
        if (existingRating == null)
        {
            return Result<bool>.Failure($"User with {userId} hasn`t already rated this course.");
        }
        existingRating.Rating = ratingDTO.Rating;

        await _ratingRepository.UpdateRatingAsync(existingRating);

        return Result<bool>.Success(true);
    }
}

using CourseService.Application.DTO;
using CourseService.Application.Extentions;
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

    public async Task<Result<CourseRatingResponse>> CreateRatingAsync(CourseRatingDTO ratingDTO, Guid courseId, Guid userId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if (course == null)
        {
            return Result<CourseRatingResponse>.Failure($"Course with {courseId} not found");
        }

        var exists = await _ratingRepository.GetRatingByCourseIdAndUserIdAsync(courseId, userId);   
        if(exists != null)
        {
            return Result<CourseRatingResponse>.Failure($"User with {userId} has already rated this course.");
        }

        var rating = new CourseRating
        {
            CourseId = courseId,
            UserId = userId,
            Rating = ratingDTO.Rating,
        };
        await _ratingRepository.CreateRatingAsync(rating);

        //course.CourseRatings.Add(rating);
        var (average, amount) = CourseRatingExtensions.CalculateRating(course.CourseRatings);


        return Result<CourseRatingResponse>.Success(
            new CourseRatingResponse
            {
                AverageRating = average,
                AmountRatings = amount
            });
    }

    public async Task<Result<CourseRatingResponse>> UpdateRatingAsync(CourseRatingDTO ratingDTO, Guid courseId, Guid userId)
    {
        var rating = await _ratingRepository.GetRatingByCourseIdAndUserIdAsync(courseId, userId);
        if (rating == null)
        {
            return Result<CourseRatingResponse>.Failure($"User with {userId} hasn`t already rated this course.");
        }

        rating.Rating = ratingDTO.Rating;
        await _ratingRepository.UpdateRatingAsync(rating);

        var ratings = await _ratingRepository.GetRatingsByCourseIdAsync(courseId);
        var (average, amount) = CourseRatingExtensions.CalculateRating(ratings);

        return Result<CourseRatingResponse>.Success(
            new CourseRatingResponse
            {
                AverageRating = average,
                AmountRatings = amount
            });
    }
}

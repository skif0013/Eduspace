using CourseService.Application.Caching;
using CourseService.Application.Courses.DTO;
using CourseService.Application.Courses.Errors;
using CourseService.Application.Courses.Interfaces;
using CourseService.Application.Courses.Services;
using CourseService.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CourseService.UnitTests.Application.Services;

public class CourseRatingServiceTests
{
    private readonly Mock<ICourseRatingRepository> _ratingRepositoryMock = new();
    private readonly Mock<ICourseRepository> _courseRepositoryMock = new();
    private readonly Mock<ICourseCache> _cacheMock = new();
    private readonly Mock<ILogger<CourseRatingService>> _loggerMock = new();
    private readonly Mock<IRedisKeyBuilder> _redisKeyBuilderMock = new();

    private readonly ICourseRatingService _courseRatingService;

    public CourseRatingServiceTests()
    {
        _courseRatingService = new CourseRatingService(
            _ratingRepositoryMock.Object,
            _courseRepositoryMock.Object,
            _cacheMock.Object,
            _loggerMock.Object,
            _redisKeyBuilderMock.Object);
    }

    [Fact]
    public async Task CreateRatingAsync_ShouldReturnCourseNotFound_WhenCourseDoesNotExist()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var courseRatingDto = new CourseRatingDTO { Rating = 1 };

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync((Course?)null);

        // Act
        var result = await _courseRatingService.CreateRatingAsync(courseRatingDto, courseId, userId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.CourseNotFound);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _ratingRepositoryMock.Verify(x => x.GetRatingByCourseIdAndUserIdAsync(courseId, userId), Times.Never());
        _ratingRepositoryMock.Verify(x => x.CreateRatingAsync(It.IsAny<CourseRating>()), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task CreateRatingAsync_ShouldReturnRatingAlreadyExists_WhenUserAlreadyRatedCourse()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var courseRatingDto = new CourseRatingDTO
        {
            Rating = 1,
        };

        var course = new Course
        {
            Id = courseId,
            CourseRatings = new List<CourseRating>()
        };

        var existingRating = new CourseRating
        {
            CourseId = courseId,
            UserId = userId
        };

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        _ratingRepositoryMock
            .Setup(x => x.GetRatingByCourseIdAndUserIdAsync(courseId, userId))
            .ReturnsAsync(existingRating);

        // Act
        var result = await _courseRatingService.CreateRatingAsync(courseRatingDto, courseId, userId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseRatingErrors.RatingAlreadyExists);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());
        _ratingRepositoryMock.Verify(x => x.GetRatingByCourseIdAndUserIdAsync(courseId, userId), Times.Once());

        _ratingRepositoryMock.Verify(x => x.CreateRatingAsync(It.IsAny<CourseRating>()), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task CreateRatingAsync_ShouldRateCourseAndReturnSuccess_WhenUserHasNotRatedCourse()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var dto = new CourseRatingDTO
        {
            Rating = 4
        };

        var course = new Course
        {
            Id = courseId,
            CourseRatings = new List<CourseRating>
            {
                new CourseRating { Rating = 3 },
                new CourseRating { Rating = 5 }
            }
        };

        var courseKey = "test-key";

        const double expectedAverageRating = 4.0;
        const int expectedAmountRating = 3;

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        _ratingRepositoryMock
            .Setup(x => x.GetRatingByCourseIdAndUserIdAsync(courseId, userId))
            .ReturnsAsync((CourseRating?)null);

        _redisKeyBuilderMock
            .Setup(x => x.GetCourseKey(courseId))
            .Returns(courseKey);

        // Act 
        var result = await _courseRatingService.CreateRatingAsync(dto, courseId, userId);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.AverageRating.Should().Be(expectedAverageRating);
        result.Value.AmountRatings.Should().Be(expectedAmountRating);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());
        _ratingRepositoryMock.Verify(x => x.GetRatingByCourseIdAndUserIdAsync(courseId, userId), Times.Once());
        _ratingRepositoryMock.Verify(x => x.CreateRatingAsync(It.IsAny<CourseRating>()), Times.Once());
        _redisKeyBuilderMock.Verify(x => x.GetCourseKey(courseId), Times.Once());
        _cacheMock.Verify(x => x.RemoveAsync(courseKey), Times.Once());
    }
}

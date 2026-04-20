using AutoMapper;
using CourseService.Application.Caching;
using CourseService.Application.Courses.DTO;
using CourseService.Application.Courses.Errors;
using CourseService.Application.Courses.Interfaces;
using CourseService.Application.Messaging;
using CourseService.Domain.Entities;
using CourseService.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CourseService.UnitTests.Application.Services.CourseServece;

public class UpdateCourseTests
{
    private readonly Mock<ICourseRepository> _courseRepositoryMock = new();
    private readonly Mock<ICourseCache> _cacheMock = new();
    private readonly Mock<ILogger<CourseService.Application.Courses.Services.CourseService>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IMessagePublisher> _publisherMock = new();
    private readonly Mock<IRedisKeyBuilder> _keyBuilderMock = new();

    private readonly CourseService.Application.Courses.Services.CourseService _courseService; // SUT

    public UpdateCourseTests()
    {
        _courseService = new CourseService.Application.Courses.Services.CourseService(
            _courseRepositoryMock.Object,
            _cacheMock.Object,
            _loggerMock.Object,
            _mapperMock.Object,
            _publisherMock.Object,
            _keyBuilderMock.Object
        );
    }

    [Fact]
    public async Task UpdateCourseAsync_ShouldReturnCourseNotFound_WhenCourseDoesNotExist()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var courseDto = new CourseDTO();

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync((Course?)null);

        // Act 
        var result = await _courseService.UpdateCourseAsync(courseDto, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.CourseNotFound);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _courseRepositoryMock.Verify(x => x.UpdateCourseAsync(It.IsAny<Course>()), Times.Never());
        _mapperMock.Verify(x => x.Map<CourseResponse>(It.IsAny<Course>()), Times.Never());
        _cacheMock.Verify(x => x.IncrementCatalogVersionAsync(), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task UpdateCourseAsync_ShouldReturnNotCourseAuthor_WhenNotOwner()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var courseDto = new CourseDTO
        {
            Name = "Ignored",
            Description = "Ignored",
            Price = 1
        };

        var anotherAuthorId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            AuthorId = anotherAuthorId,
            Status = CourseStatus.Published
        };

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        // Act 
        var result = await _courseService.UpdateCourseAsync(courseDto, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.NotCourseAuthor);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _courseRepositoryMock.Verify(x => x.UpdateCourseAsync(It.IsAny<Course>()), Times.Never());
        _mapperMock.Verify(x => x.Map<CourseResponse>(It.IsAny<Course>()), Times.Never());
        _cacheMock.Verify(x => x.IncrementCatalogVersionAsync(), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task UpdateCourseAsync_ShouldReturnCourseArchived_WhenCourseIsArchived()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var courseDto = new CourseDTO
        {
            Name = "Ignored",
            Description = "Ignored",
            Price = 1
        };

        var course = new Course
        {
            Id = courseId,
            AuthorId = authorId,
            Status = CourseStatus.Archived
        };

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await _courseService.UpdateCourseAsync(courseDto, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.CourseArchived);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _courseRepositoryMock.Verify(x => x.UpdateCourseAsync(It.IsAny<Course>()), Times.Never());
        _mapperMock.Verify(x => x.Map<CourseResponse>(It.IsAny<Course>()), Times.Never());
        _cacheMock.Verify(x => x.IncrementCatalogVersionAsync(), Times.Never());
        _keyBuilderMock.Verify(x => x.GetCourseKey(It.IsAny<Guid>()), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task UpdateCourseAsync_ShouldUpdateCourse_WhenUserIsOwnerAndCourseIsNotArchived()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var courseDto = new CourseDTO
        {
            Name = "Updated-Name",
            Description = "Updated-Description",
            Price = 2,
            IsFree = true,
            AvatarURL = "Updated-url"
        };

        var course = new Course
        {
            Id = courseId,
            AuthorId = authorId,
            Name = "Name",
            Description = "Description",
            Price = 1,
            IsFree = false,
            AvatarURL = "url",
            CourseRatings = new List<CourseRating>
            {
                new CourseRating { Rating = 1 },
                new CourseRating { Rating = 5 }
            }
        };

        var cacheKey = "test-key";

        const double expectedAverage = 3.0;
        const int expectedRatingsCount = 2;

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        _mapperMock
            .Setup(x => x.Map<CourseResponse>(It.IsAny<Course>()))
            .Returns(new CourseResponse());

        _keyBuilderMock
            .Setup(x => x.GetCourseKey(courseId))
            .Returns(cacheKey);

        // Act
        var result = await _courseService.UpdateCourseAsync(courseDto, courseId, authorId);

        // Assert
        result.IsSuccess.Should().BeTrue();

        course.Name.Should().BeEquivalentTo(courseDto.Name);
        course.Description.Should().BeEquivalentTo(courseDto.Description);
        course.Price.Should().Be(courseDto.Price);
        course.IsFree.Should().Be(courseDto.IsFree);
        course.AvatarURL.Should().Be(courseDto.AvatarURL);

        result.Value.AverageRating.Should().Be(expectedAverage);
        result.Value.AmountRatings.Should().Be(expectedRatingsCount);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());
        _courseRepositoryMock.Verify(x => x.UpdateCourseAsync(course), Times.Once());
        _mapperMock.Verify(x => x.Map<CourseResponse>(course), Times.Once());
        _cacheMock.Verify(x => x.IncrementCatalogVersionAsync(), Times.Once());
        _keyBuilderMock.Verify(x => x.GetCourseKey(courseId), Times.Once());
        _cacheMock.Verify(x => x.RemoveAsync(cacheKey), Times.Once());
    }
}

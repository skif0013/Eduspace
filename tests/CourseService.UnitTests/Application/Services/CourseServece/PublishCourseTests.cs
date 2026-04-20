using AutoMapper;
using CourseService.Application.Caching;
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

public class PublishCourseTests
{
    private readonly Mock<ICourseRepository> _courseRepositoryMock = new();
    private readonly Mock<ICourseCache> _cacheMock = new();
    private readonly Mock<ILogger<CourseService.Application.Courses.Services.CourseService>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IMessagePublisher> _publisherMock = new();
    private readonly Mock<IRedisKeyBuilder> _keyBuilderMock = new();

    private readonly CourseService.Application.Courses.Services.CourseService _courseService; // SUT

    public PublishCourseTests()
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
    public async Task PublishCourseAsync_ShouldReturnCourseNotFound_WhenCourseDoesNotExist()
    {
        // Arrage
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync((Course?)null);

        // Act 
        var result = await _courseService.PublishCourseAsync(courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.CourseNotFound);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _courseRepositoryMock.Verify(x => x.UpdateCourseAsync(It.IsAny<Course>()), Times.Never());
        _cacheMock.Verify(x => x.IncrementCatalogVersionAsync(), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
        _publisherMock.Verify(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task PublishCourseAsync_ShouldReturnNotCourseAuthor_WhenUserIsNotCourseAuthor()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var anotherAuthorId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            AuthorId = anotherAuthorId,
            Status = CourseStatus.Draft
        };

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await _courseService.PublishCourseAsync(courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.NotCourseAuthor);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _courseRepositoryMock.Verify(x => x.UpdateCourseAsync(It.IsAny<Course>()), Times.Never());
        _cacheMock.Verify(x => x.IncrementCatalogVersionAsync(), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
        _publisherMock.Verify(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task PublishCourseAsync_ShouldSetStatusToPublished_WhenDataIsValid()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var course = new Course
        {
            Id = courseId,
            AuthorId = authorId,
            Status = CourseStatus.Draft
        };

        var cacheKey = "test-key";

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        _keyBuilderMock
            .Setup(x => x.GetCourseKey(courseId))
            .Returns(cacheKey);

        // Act
        var result = await _courseService.PublishCourseAsync(courseId, authorId);

        // Assert
        result.IsSuccess.Should().BeTrue();

        course.Status.Should().Be(CourseStatus.Published);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());
        _courseRepositoryMock.Verify(x => x.UpdateCourseAsync(course), Times.Once());
        _cacheMock.Verify(x => x.IncrementCatalogVersionAsync(), Times.Once());
        _keyBuilderMock.Verify(x => x.GetCourseKey(courseId), Times.Once());
        _cacheMock.Verify(x => x.RemoveAsync(cacheKey), Times.Once());

        _publisherMock.Verify(x => x.PublishAsync("course.published",
            It.Is<string>(json =>
                json.Contains(courseId.ToString()) &&
                json.Contains(authorId.ToString()))
            ), Times.Once());
    }
}

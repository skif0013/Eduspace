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

namespace CourseService.UnitTests.Application.Services.LessonService;

public class DeleteLessonsTests
{
    private readonly Mock<ILessonRepository> _lessonRepositoryMock = new();
    private readonly Mock<ICourseRepository> _courseRepositoryMock = new();
    private readonly Mock<ICourseCache> _cacheMock = new();
    private readonly Mock<ILogger<CourseService.Application.Courses.Services.LessonService>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IMessagePublisher> _publisherMock = new();
    private readonly Mock<IRedisKeyBuilder> _keyBuilderMock = new();

    private readonly CourseService.Application.Courses.Services.LessonService _lessonService; // SUT

    public DeleteLessonsTests()
    {
        _lessonService = new CourseService.Application.Courses.Services.LessonService(
            _courseRepositoryMock.Object,
            _cacheMock.Object,
            _lessonRepositoryMock.Object,
            _loggerMock.Object,
            _mapperMock.Object,
            _publisherMock.Object,
            _keyBuilderMock.Object
        );
    }

    [Fact]
    public async Task DeleteLessonAsync_ShouldReturnCourseNotFound_WhenCourseDoesNotExist()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId!))
            .ReturnsAsync((Course?)null);

        // Act
        var result = await _lessonService.DeleteLessonAsync(lessonId, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.CourseNotFound);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _lessonRepositoryMock.Verify(x => x.GetLessonByIdAsync(It.IsAny<Guid>()), Times.Never());
        _lessonRepositoryMock.Verify(x => x.DeleteLessonAsync(It.IsAny<Lesson>()), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task DeleteLessonAsync_ShouldReturnNotCourseAuthor_WhenUserIsNotCourseAuthor()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var anotherAuthorId = Guid.NewGuid();
        var course = new Course
        {
            Id = lessonId,
            AuthorId = anotherAuthorId,
            Status = CourseStatus.Published
        };

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await _lessonService.DeleteLessonAsync(lessonId, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.NotCourseAuthor);
        anotherAuthorId.Should().NotBe(authorId);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _lessonRepositoryMock.Verify(x => x.GetLessonByIdAsync(It.IsAny<Guid>()), Times.Never());
        _lessonRepositoryMock.Verify(x => x.DeleteLessonAsync(It.IsAny<Lesson>()), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task DeleteLessonAsync_ShouldReturnCourseArchived_WhenCourseIsArchived()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

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
        var result = await _lessonService.DeleteLessonAsync(lessonId, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.CourseArchived);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _lessonRepositoryMock.Verify(x => x.GetLessonByIdAsync(It.IsAny<Guid>()), Times.Never());
        _lessonRepositoryMock.Verify(x => x.DeleteLessonAsync(It.IsAny<Lesson>()), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task DeleteLessonAsync_ShouldReturnLessonNotFound_WhenLessonDoesNotExist()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var course = new Course
        {
            Id = courseId,
            AuthorId = authorId,
            Status = CourseStatus.Published
        };

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        _lessonRepositoryMock
            .Setup(x => x.GetLessonByIdAsync(lessonId))
            .ReturnsAsync((Lesson?)null);

        // Act
        var result = await _lessonService.DeleteLessonAsync(lessonId, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LessonErrors.LessonNotFound);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());
        _lessonRepositoryMock.Verify(x => x.GetLessonByIdAsync(lessonId), Times.Once());

        _lessonRepositoryMock.Verify(x => x.DeleteLessonAsync(It.IsAny<Lesson>()), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task DeleteLessonAsync_ShouldReturnLessonNotInCourse_WhenLessonDoesNotBelongToCourse()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var course = new Course
        {
            Id = courseId,
            AuthorId = authorId,
            Status = CourseStatus.Published
        };

        var anotherCourseId = Guid.NewGuid();
        var lesson = new Lesson
        {
            Id = lessonId,
            CourseId = anotherCourseId,
        };

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        _lessonRepositoryMock
            .Setup(x => x.GetLessonByIdAsync(lessonId))
            .ReturnsAsync(lesson);

        // Act
        var result = await _lessonService.DeleteLessonAsync(lessonId, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LessonErrors.LessonNotFound);
        anotherCourseId.Should().NotBe(courseId);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());
        _lessonRepositoryMock.Verify(x => x.GetLessonByIdAsync(lessonId), Times.Once());

        _lessonRepositoryMock.Verify(x => x.DeleteLessonAsync(It.IsAny<Lesson>()), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task DeleteLessonAsync_ShouldDeleteLesson_WhenLessonExistsInCourse()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var cacheKey = "test-key";

        var course = new Course
        {
            Id = courseId,
            AuthorId = authorId,
            Status = CourseStatus.Published
        };

        var lesson = new Lesson
        {
            Id = lessonId,
            CourseId = courseId,
            LessonNumber = 1,
            Name = "Test Lesson",
            Description = "Test Description",
            VideoUrl = "test-url",
            CreatedAt = DateTime.UtcNow,
        };

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        _lessonRepositoryMock
            .Setup(x => x.GetLessonByIdAsync(lessonId))
            .ReturnsAsync(lesson);

        _keyBuilderMock
            .Setup(x => x.GetCourseKey(courseId))
            .Returns(cacheKey);

        // Act 
        var result = await _lessonService.DeleteLessonAsync(lessonId, courseId, authorId);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());
        _lessonRepositoryMock.Verify(x => x.GetLessonByIdAsync(lessonId), Times.Once());
        _lessonRepositoryMock.Verify(x => x.DeleteLessonAsync(lesson), Times.Once());
        _cacheMock.Verify(x => x.RemoveAsync(cacheKey), Times.Once());
    }
}

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

namespace CourseService.UnitTests.Application.Services.LessonService;

public class CreateLessonTests
{
    private readonly Mock<ILessonRepository> _lessonRepositoryMock = new();
    private readonly Mock<ICourseRepository> _courseRepositoryMock = new();
    private readonly Mock<ICourseCache> _cacheMock = new();
    private readonly Mock<ILogger<CourseService.Application.Courses.Services.LessonService>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IMessagePublisher> _publisherMock = new();
    private readonly Mock<IRedisKeyBuilder> _keyBuilderMock = new();

    private readonly CourseService.Application.Courses.Services.LessonService _lessonService; // SUT

    public CreateLessonTests()
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
    public async Task CreateLessonAsync_ShouldReturnCourseNotFound_WhenCourseDoesNotExist()
    {
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var lessonDto = new LessonDTO();

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync((Course?)null);

        // Act
        var result = await _lessonService.CreateLessonAsync(lessonDto, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.CourseNotFound);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _mapperMock.Verify(x => x.Map<Lesson>(It.IsAny<LessonDTO>), Times.Never());
        _lessonRepositoryMock.Verify(x => x.CreateLessonAsync(It.IsAny<Lesson>()), Times.Never());
        _mapperMock.Verify(x => x.Map<LessonResponse>(It.IsAny<Lesson>()), Times.Never());
        _publisherMock.Verify(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task CreateLessonAsync_ShouldReturnNotCourseAuthor_WhenUserIsNotCourseAuthor()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var lessonDto = new LessonDTO();

        var course = new Course
        {
            Id = courseId,
            AuthorId = Guid.NewGuid(),
            Status = CourseStatus.Published
        };

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await _lessonService.CreateLessonAsync(lessonDto, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.NotCourseAuthor);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _mapperMock.Verify(x => x.Map<Lesson>(It.IsAny<LessonDTO>()), Times.Never());
        _lessonRepositoryMock.Verify(x => x.CreateLessonAsync(It.IsAny<Lesson>()), Times.Never());
        _mapperMock.Verify(x => x.Map<LessonResponse>(It.IsAny<Lesson>()), Times.Never());
        _publisherMock.Verify(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task CreateLessonAsync_ShouldReturnCourseArchived_WhenCourseIsArchived()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var lessonDto = new LessonDTO();

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
        var result = await _lessonService.CreateLessonAsync(lessonDto, courseId, authorId);

        // Assert

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.CourseArchived);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _mapperMock.Verify(x => x.Map<Lesson>(It.IsAny<LessonDTO>()), Times.Never());
        _lessonRepositoryMock.Verify(x => x.CreateLessonAsync(It.IsAny<Lesson>()), Times.Never());
        _mapperMock.Verify(x => x.Map<LessonResponse>(It.IsAny<Lesson>()), Times.Never());
        _publisherMock.Verify(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task CreateLessonAsync_ShouldCreateLesson_WhenUserIsCourseAuthorAndDataIsValid()
    {
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var lessonDto = new LessonDTO();

        var course = new Course
        {
            Id = courseId,
            AuthorId = authorId,
            Status = CourseStatus.Draft
        };

        var lesson = new Lesson
        {
            Id = Guid.NewGuid(),
        };

        var lessonResponse = new LessonResponse();

        string cacheKey = "test-key";

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        _mapperMock
            .Setup(x => x.Map<Lesson>(lessonDto))
            .Returns(lesson);

        _lessonRepositoryMock
            .Setup(x => x.CreateLessonAsync(lesson))
            .ReturnsAsync(lesson);

        _mapperMock
            .Setup(x => x.Map<LessonResponse>(lesson))
            .Returns(lessonResponse);

        _keyBuilderMock
            .Setup(x => x.GetCourseKey(courseId))
            .Returns(cacheKey);

        // Act 
        var result = await _lessonService.CreateLessonAsync(lessonDto, courseId, authorId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(lessonResponse);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _mapperMock.Verify(x => x.Map<Lesson>(lessonDto), Times.Once());
        _lessonRepositoryMock.Verify(x => x.CreateLessonAsync(lesson), Times.Once());
        _mapperMock.Verify(x => x.Map<LessonResponse>(lesson), Times.Once());

        _publisherMock.Verify(x => x.PublishAsync("lesson.created", It.IsAny<string>()), Times.Once());

        _keyBuilderMock.Verify(x => x.GetCourseKey(courseId), Times.Once());
        _cacheMock.Verify(x => x.RemoveAsync(cacheKey), Times.Once());
    }
}

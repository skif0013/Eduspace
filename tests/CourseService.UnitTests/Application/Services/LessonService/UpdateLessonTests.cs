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

public class UpdateLessonTests
{
    private readonly Mock<ILessonRepository> _lessonRepositoryMock = new();
    private readonly Mock<ICourseRepository> _courseRepositoryMock = new();
    private readonly Mock<ICourseCache> _cacheMock = new();
    private readonly Mock<ILogger<CourseService.Application.Courses.Services.LessonService>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IMessagePublisher> _publisherMock = new();
    private readonly Mock<IRedisKeyBuilder> _keyBuilderMock = new();

    private readonly CourseService.Application.Courses.Services.LessonService _lessonService; // SUT

    public UpdateLessonTests()
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
    public async Task UpdateLessonAsync_ShouldReturnCourseNotFound_WhenCourseDoesNotExist()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var lessonDto = new LessonDTO();

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync((Course?)null);

        // Act
        var result = await _lessonService.UpdateLessonAsync(lessonDto, lessonId, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.CourseNotFound);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _lessonRepositoryMock.Verify(x => x.GetLessonByIdAsync(It.IsAny<Guid>()), Times.Never());
        _lessonRepositoryMock.Verify(x => x.UpdateLessonAsync(It.IsAny<Lesson>()), Times.Never());
        _mapperMock.Verify(x => x.Map<LessonResponse>(It.IsAny<Lesson>()), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task UpdateLessonAsync_ShouldReturnNotCourseAuthor_WhenUserIsNotCourseAuthor()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var lessonDto = new LessonDTO();

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
        var result = await _lessonService.UpdateLessonAsync(lessonDto, lessonId, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.NotCourseAuthor);
        anotherAuthorId.Should().NotBe(authorId);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _lessonRepositoryMock.Verify(x => x.GetLessonByIdAsync(It.IsAny<Guid>()), Times.Never());
        _lessonRepositoryMock.Verify(x => x.UpdateLessonAsync(It.IsAny<Lesson>()), Times.Never());
        _mapperMock.Verify(x => x.Map<LessonResponse>(It.IsAny<Lesson>()), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task UpdateLessonAsync_ShouldReturnCourseArchived_WhenCourseIsArchived()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
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
        var result = await _lessonService.UpdateLessonAsync(lessonDto, lessonId, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.CourseArchived);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _lessonRepositoryMock.Verify(x => x.GetLessonByIdAsync(It.IsAny<Guid>()), Times.Never());
        _lessonRepositoryMock.Verify(x => x.UpdateLessonAsync(It.IsAny<Lesson>()), Times.Never());
        _mapperMock.Verify(x => x.Map<LessonResponse>(It.IsAny<Lesson>()), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task UpdateLessonAsync_ShouldReturnLessonNotFound_WhenLessonDoesNotExist()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var lessonDto = new LessonDTO();

        var course = new Course
        {
            Id = courseId,
            AuthorId = authorId,
            Status = CourseStatus.Draft
        };

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        _lessonRepositoryMock
            .Setup(x => x.GetLessonByIdAsync(lessonId))
            .ReturnsAsync((Lesson?)null);

        // Act
        var result = await _lessonService.UpdateLessonAsync(lessonDto, lessonId, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LessonErrors.LessonNotFound);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());
        _lessonRepositoryMock.Verify(x => x.GetLessonByIdAsync(lessonId), Times.Once());

        _lessonRepositoryMock.Verify(x => x.UpdateLessonAsync(It.IsAny<Lesson>()), Times.Never());
        _mapperMock.Verify(x => x.Map<LessonResponse>(It.IsAny<Lesson>()), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task UpdateLessonAsync_ShouldReturnLessonNotFound_WhenLessonNotInCourse()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var lessonDto = new LessonDTO();

        var course = new Course
        {
            Id = courseId,
            AuthorId = authorId,
            Status = CourseStatus.Draft
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
        var result = await _lessonService.UpdateLessonAsync(lessonDto, lessonId, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LessonErrors.LessonNotFound);
        anotherCourseId.Should().NotBe(courseId);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());
        _lessonRepositoryMock.Verify(x => x.GetLessonByIdAsync(lessonId), Times.Once());

        _lessonRepositoryMock.Verify(x => x.UpdateLessonAsync(It.IsAny<Lesson>()), Times.Never());
        _mapperMock.Verify(x => x.Map<LessonResponse>(It.IsAny<Lesson>()), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task UpdateLessonAsync_ShouldUpdateLesson_WhenLessonBelongsToCourse()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var lessonDto = new LessonDTO
        {
            LessonNumber = 2,
            Name = "Updated name",
            Description = "Updated description",
            VideoUrl = "new-url"
        };

        var course = new Course
        {
            Id = courseId,
            AuthorId = authorId,
            Status = CourseStatus.Draft
        };

        var lesson = new Lesson
        {
            Id = lessonId,
            CourseId = courseId,
            LessonNumber = 1,
            Name = "Name",
            Description = "Description",
            VideoUrl = "test-url"
        };

        var expectedResponse = new LessonResponse
        {
            Id = lesson.Id,
            LessonNumber = lessonDto.LessonNumber,
            Name = lessonDto.Name,
            Description = lessonDto.Description,
            VideoUrl = lessonDto.VideoUrl
        };

        var cacheKey = "test-key";

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        _lessonRepositoryMock
            .Setup(x => x.GetLessonByIdAsync(lessonId))
            .ReturnsAsync(lesson);

        _lessonRepositoryMock
            .Setup(x => x.UpdateLessonAsync(lesson));

        _mapperMock
            .Setup(x => x.Map<LessonResponse>(lesson))
            .Returns(expectedResponse);

        _keyBuilderMock
            .Setup(x => x.GetCourseKey(courseId))
            .Returns(cacheKey);

        // Act
        var result = await _lessonService.UpdateLessonAsync(lessonDto, lessonId, courseId, authorId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedResponse);
        lesson.LessonNumber.Should().Be(lessonDto.LessonNumber);
        lesson.Name.Should().Be(lessonDto.Name);
        lesson.Description.Should().Be(lessonDto.Description);
        lesson.VideoUrl.Should().Be(lessonDto.VideoUrl);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());
        _lessonRepositoryMock.Verify(x => x.GetLessonByIdAsync(lessonId), Times.Once());
        _lessonRepositoryMock.Verify(x => x.UpdateLessonAsync(It.Is<Lesson>(l =>
            l.LessonNumber == lessonDto.LessonNumber &&
            l.Name == lessonDto.Name &&
            l.Description == lessonDto.Description &&
            l.VideoUrl == lessonDto.VideoUrl)), Times.Once());

        _mapperMock.Verify(x => x.Map<LessonResponse>(It.Is<Lesson>(l =>
            l.LessonNumber == lessonDto.LessonNumber &&
            l.Name == lessonDto.Name &&
            l.Description == lessonDto.Description &&
            l.VideoUrl == lessonDto.VideoUrl
        )), Times.Once());

        _cacheMock.Verify(x => x.RemoveAsync(cacheKey), Times.Once());
    }
}

using AutoMapper;
using CourseService.Application.Caching;
using CourseService.Application.Courses.DTO;
using CourseService.Application.Courses.Errors;
using CourseService.Application.Courses.Interfaces;
using CourseService.Application.Messaging;
using CourseService.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CourseService.UnitTests.Application.Services.LessonService;

public class GetLessonByIdTests
{
    private readonly Mock<ILessonRepository> _lessonRepositoryMock = new();
    private readonly Mock<ICourseRepository> _courseRepositoryMock = new();
    private readonly Mock<ICourseCache> _cacheMock = new();
    private readonly Mock<ILogger<CourseService.Application.Courses.Services.LessonService>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IMessagePublisher> _publisherMock = new();
    private readonly Mock<IRedisKeyBuilder> _keyBuilderMock = new();

    private readonly CourseService.Application.Courses.Services.LessonService _lessonService; // SUT

    public GetLessonByIdTests()
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
    public async Task GetLessonByIdAsync_ShouldReturnLessonNotFound_WhenLessonDoesNotExist()
    {
        // Arrange
        var lessonId = Guid.NewGuid();

        _lessonRepositoryMock
            .Setup(x => x.GetLessonByIdAsync(lessonId))
            .ReturnsAsync((Lesson?)null);

        // Act
        var result = await _lessonService.GetLessonByIdAsync(lessonId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LessonErrors.LessonNotFound);

        _lessonRepositoryMock.Verify(x => x.GetLessonByIdAsync(lessonId), Times.Once());
        _mapperMock.Verify(x => x.Map<LessonResponse>(It.IsAny<Lesson>()), Times.Never());
    }

    [Fact]
    public async Task GetLessonByIdAsync_ShouldReturnLesson_WhenLessonExists()
    {
        // Arrange
        var lessonId = Guid.NewGuid();

        var lesson = new Lesson
        {
            Id = lessonId,
            LessonNumber = 1,
            Name = "Test Lesson",
            Description = "Test Description",
            VideoUrl = "test-url",
            CreatedAt = DateTime.UtcNow,
        };

        var expectedResponse = new LessonResponse
        {
            Id = lesson.Id,
            LessonNumber = lesson.LessonNumber,
            Name = lesson.Name,
            Description = lesson.Description,
            VideoUrl = lesson.VideoUrl,
            CreatedAt = lesson.CreatedAt,
        };

        _lessonRepositoryMock
            .Setup(x => x.GetLessonByIdAsync(lessonId))
            .ReturnsAsync(lesson);

        _mapperMock
            .Setup(x => x.Map<LessonResponse>(It.IsAny<Lesson>()))
            .Returns(expectedResponse);

        // Act
        var result = await _lessonService.GetLessonByIdAsync(lessonId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedResponse);

        _lessonRepositoryMock.Verify(x => x.GetLessonByIdAsync(lessonId), Times.Once());
        _mapperMock.Verify(x => x.Map<LessonResponse>(lesson), Times.Once());
    }
}

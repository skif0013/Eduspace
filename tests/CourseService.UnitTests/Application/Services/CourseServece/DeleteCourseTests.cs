using AutoMapper;
using BuildingBlock.UserContextMiddleware.Models;
using CourseService.Application.Caching;
using CourseService.Application.Courses.Errors;
using CourseService.Application.Courses.Interfaces;
using CourseService.Application.Messaging;
using CourseService.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CourseService.UnitTests.Application.Services.CourseServece;

public class DeleteCourseTests
{
    private readonly Mock<ICourseRepository> _courseRepositoryMock = new();
    private readonly Mock<ICourseCache> _cacheMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IMessagePublisher> _publisherMock = new();
    private readonly Mock<IRedisKeyBuilder> _keyBuilderMock = new();
    private readonly UserContext _userContext = new();
    private readonly CourseService.Application.Courses.Services.CourseService _courseService;

    public DeleteCourseTests()
    {
        _courseService = new CourseService.Application.Courses.Services.CourseService(
            _userContext,
            _courseRepositoryMock.Object,
            _cacheMock.Object,
            Mock.Of<ILogger<CourseService.Application.Courses.Services.CourseService>>(),
            _mapperMock.Object,
            _publisherMock.Object,
            _keyBuilderMock.Object);
    }

    [Fact]
    public async Task DeleteCourseAsync_ShouldDeleteCourse_WhenCurrentUserIsAuthor()
    {
        var courseId = Guid.NewGuid();
        var cacheKey = "course-key";
        var course = new Course { Id = courseId, AuthorId = Guid.NewGuid() };
        _userContext.UserId = course.AuthorId;

        _courseRepositoryMock.Setup(x => x.GetCourseByIdAsync(courseId)).ReturnsAsync(course);
        _courseRepositoryMock.Setup(x => x.DeleteCourseAsync(courseId)).ReturnsAsync(course);
        _keyBuilderMock.Setup(x => x.GetCourseKey(courseId)).Returns(cacheKey);

        var result = await _courseService.DeleteCourseAsync(courseId);

        result.IsSuccess.Should().BeTrue();
        _courseRepositoryMock.Verify(x => x.DeleteCourseAsync(courseId), Times.Once);
        _cacheMock.Verify(x => x.IncrementCatalogVersionAsync(), Times.Once);
        _cacheMock.Verify(x => x.RemoveAsync(cacheKey), Times.Once);
    }

    [Fact]
    public async Task DeleteCourseAsync_ShouldRejectDeletion_WhenCurrentUserIsNotAuthor()
    {
        var courseId = Guid.NewGuid();
        _userContext.UserId = Guid.NewGuid();
        var course = new Course { Id = courseId, AuthorId = Guid.NewGuid() };

        _courseRepositoryMock.Setup(x => x.GetCourseByIdAsync(courseId)).ReturnsAsync(course);

        var result = await _courseService.DeleteCourseAsync(courseId);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.NotCourseAuthor);
        _courseRepositoryMock.Verify(x => x.DeleteCourseAsync(It.IsAny<Guid>()), Times.Never);
        _cacheMock.Verify(x => x.IncrementCatalogVersionAsync(), Times.Never);
    }
}

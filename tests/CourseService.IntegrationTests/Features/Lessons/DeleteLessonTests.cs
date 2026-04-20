using CourseService.Application.Courses.Errors;
using CourseService.Domain.Entities;
using CourseService.Domain.Enums;
using CourseService.Infrastructure.Data;
using CourseService.IntegrationTests.Common;
using CourseService.IntegrationTests.Common.Fixtures;
using CourseService.IntegrationTests.Common.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace CourseService.IntegrationTests.Features.Lessons;

[Collection("Postgres collection")]
public class DeleteLessonTests
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public DeleteLessonTests(PostgresContainerFixture postgres)
    {
        _factory = new TestWebApplicationFactory(postgres);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task ShouldReturnNoContent_WhenLessonDeleted()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var authorId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        using (var arrangeScope = _factory.Services.CreateScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var course = new Course
            {
                Id = courseId,
                Name = "Test Name",
                Description = "Test Description",
                Price = 0,
                IsFree = true,
                AuthorId = authorId,
                Status = CourseStatus.Published
            };

            db.Courses.Add(course);

            var lesson = new Lesson
            {
                Id = lessonId,
                CourseId = courseId,
                LessonNumber = 1,
                Name = "Test Name",
                Description = "Test Description",
                VideoUrl = "http://test.com"
            };

            db.Lessons.Add(lesson);

            await db.SaveChangesAsync();
        }

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Delete, $"/api/courses/{courseId}/lessons/{lessonId}", authorId);


        // Act
        var response = await _client.SendAsync(request);

        // Assert HTTP
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert DB
        using (var assertScope = _factory.Services.CreateScope())
        {
            var db = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var deletedLesson = await db.Lessons.FirstOrDefaultAsync(x => x.Id == lessonId);
            deletedLesson.Should().BeNull();

            var lessonsCount = await db.Lessons.CountAsync();
            lessonsCount.Should().Be(0);
        }
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenCourseDoesNotExist()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Delete, $"/api/courses/{courseId}/lessons/{lessonId}", userId);

        // Act 
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        content.Should().NotBeNull();
        content!.Status.Should().Be((int)HttpStatusCode.NotFound);
        content.Title.Should().Be(CourseErrors.CourseNotFound.Code);
        content.Detail.Should().Be(CourseErrors.CourseNotFound.Description);
    }

    [Fact]
    public async Task ShouldReturnForbidden_WhenUserIsNotCourseAuthor()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var authorId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        using (var arrangeScope = _factory.Services.CreateScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var course = new Course
            {
                Id = courseId,
                Name = "Test Name",
                Description = "Test Description",
                Price = 0,
                IsFree = true,
                AuthorId = authorId,
                Status = CourseStatus.Published
            };

            db.Courses.Add(course);

            var lesson = new Lesson
            {
                Id = lessonId,
                CourseId = courseId,
                LessonNumber = 1,
                Name = "Test Name",
                Description = "Test Description",
                VideoUrl = "http://test.com"
            };

            db.Lessons.Add(lesson);

            await db.SaveChangesAsync();
        }

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Delete, $"/api/courses/{courseId}/lessons/{lessonId}", userId);

        // Act
        var response = await _client.SendAsync(request);

        // Assert HTTP
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        content.Should().NotBeNull();
        content!.Status.Should().Be((int)HttpStatusCode.Forbidden);
        content.Title.Should().Be(CourseErrors.NotCourseAuthor.Code);
        content.Detail.Should().Be(CourseErrors.NotCourseAuthor.Description);

        // Assert DB
        using (var assertScope = _factory.Services.CreateScope())
        {
            var db = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var lesson = await db.Lessons.FirstOrDefaultAsync(x => x.Id == lessonId);
            lesson.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task ShouldReturnConflict_WhenCourseIsArchived()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        using (var arrangeScope = _factory.Services.CreateScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var course = new Course
            {
                Id = courseId,
                Name = "Test Name",
                Description = "Test Description",
                Price = 0,
                IsFree = true,
                AuthorId = userId,
                Status = CourseStatus.Archived
            };

            db.Courses.Add(course);

            var lesson = new Lesson
            {
                Id = lessonId,
                CourseId = courseId,
                LessonNumber = 1,
                Name = "Test Name",
                Description = "Test Description",
                VideoUrl = "http://test.com"
            };

            db.Lessons.Add(lesson);

            await db.SaveChangesAsync();
        }

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Delete, $"/api/courses/{courseId}/lessons/{lessonId}", userId);

        // Act
        var response = await _client.SendAsync(request);

        // Assert HTTP
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        content.Should().NotBeNull();
        content!.Status.Should().Be((int)HttpStatusCode.Conflict);
        content.Title.Should().Be(CourseErrors.CourseArchived.Code);
        content.Detail.Should().Be(CourseErrors.CourseArchived.Description);

        // Assert DB
        using (var assertScope = _factory.Services.CreateScope())
        {
            var db = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var lesson = await db.Lessons.FirstOrDefaultAsync(x => x.Id == lessonId);
            lesson.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenLessonDoesNotBelongToCourse()
    {
        // Arrange 
        await _factory.ResetDatabaseAsync();

        var authorId = Guid.NewGuid();
        var anotherCourseId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        using (var arrangeScope = _factory.Services.CreateScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var course = new Course
            {
                Id = courseId,
                AuthorId = authorId,
                Name = "Test Name",
                Description = "Test Description",
                Price = 0,
                IsFree = true,
                Status = CourseStatus.Published
            };

            var anotherCourse = new Course
            {
                Id = anotherCourseId,
                AuthorId = authorId,
                Name = "Test Name 2",
                Description = "Test Description 2",
                Price = 0,
                IsFree = true,
                Status = CourseStatus.Published
            };

            db.Courses.AddRange(course, anotherCourse);

            var lesson = new Lesson
            {
                Id = lessonId,
                CourseId = anotherCourseId,
                LessonNumber = 1,
                Name = "Test Lesson",
                Description = "Test Description",
                VideoUrl = "http://test.com"
            };

            db.Lessons.Add(lesson);

            await db.SaveChangesAsync();
        }

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Delete, $"/api/courses/{courseId}/lessons/{lessonId}", authorId);

        // Act
        var response = await _client.SendAsync(request);

        // Assert HTTP
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        content.Should().NotBeNull();
        content!.Status.Should().Be((int)HttpStatusCode.NotFound);
        content.Title.Should().Be(LessonErrors.LessonNotFound.Code);
        content.Detail.Should().Be(LessonErrors.LessonNotFound.Description);

        // Assert DB
        using (var assertScope = _factory.Services.CreateScope())
        {
            var db = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var lesson = await db.Lessons.FirstOrDefaultAsync(x => x.Id == lessonId);
            lesson.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        var request = HttpRequestFactory.CreateUnauthorized(HttpMethod.Delete, $"/api/courses/{courseId}/lessons/{lessonId}");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

using CourseService.Domain.Entities;
using CourseService.Domain.Enums;
using CourseService.Infrastructure.Data;
using CourseService.IntegrationTests.Common;
using CourseService.IntegrationTests.Common.Fixtures;
using CourseService.IntegrationTests.Common.Helpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace CourseService.IntegrationTests.Features.Courses;

[Collection("Postgres collection")]
public class PublishCourseTests
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public PublishCourseTests(PostgresContainerFixture postgres)
    {
        _factory = new TestWebApplicationFactory(postgres);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task ShouldPublishCourse_WhenRequestIsValid()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var authorId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        using (var arrangeScope = _factory.Services.CreateScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            db.Courses.Add(new Course
            {
                Id = courseId,
                AuthorId = authorId,
                Name = "Test Course",
                Description = "Test Description",
                Price = 0,
                IsFree = true,
                Status = CourseStatus.Draft
            });
            
            await db.SaveChangesAsync();
        }

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Patch, $"/api/courses/{courseId}/publish", authorId);

        // Act 
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ShouldReturnForbidden_WhenNotCourseAuthor()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var authorId = Guid.NewGuid();
        var anotherAuthorId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        using (var arrangeScope = _factory.Services.CreateScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            db.Courses.Add(new Course
            {
                Id = courseId,
                AuthorId = anotherAuthorId,
                Name = "Test Course",
                Description = "Test Description",
                Price = 0,
                IsFree = true,
                Status = CourseStatus.Draft
            });

            await db.SaveChangesAsync();
        }
        
        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Patch, $"/api/courses/{courseId}/publish", authorId);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenCourseDoesNotExist()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var authorId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Patch, $"/api/courses/{courseId}/publish", authorId);

        // Act 
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();

        var request = HttpRequestFactory.CreateUnauthorized(HttpMethod.Patch, $"/api/courses/{courseId}/publish");
        
        // Act 
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

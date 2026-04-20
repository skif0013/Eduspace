using CourseService.Application.Courses.DTO;
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

namespace CourseService.IntegrationTests.Features.CourseRatings;

[Collection("Postgres collection")]
public class CreateRatingTests
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public CreateRatingTests(PostgresContainerFixture postgres)
    {
        _factory = new TestWebApplicationFactory(postgres);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task ShouldReturnCourseRating_WhenRequestIsValid()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var dto = new CourseRatingDTO { Rating = 5 };

        using (var arrangeScope = _factory.Services.CreateScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var course = new Course
            {
                Id = courseId,
                AuthorId = authorId,
                Name = "Test Course",
                Description = "Test Description",
                Price = 0,
                IsFree = true,
                AvatarURL = "http://test.com",
                Status = CourseStatus.Published
            };

            db.Courses.Add(course);
            await db.SaveChangesAsync();
        }

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Post, $"/api/courses/{courseId}/ratings", userId, dto);

        var expectedRating = 5;
        var expectedAvatage = 1;

        // Act
        var response = await _client.SendAsync(request);

        // Assert HTTP
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<CourseRatingResponse>();

        content.Should().NotBeNull();
        content.AverageRating.Should().Be(expectedRating);
        content.AmountRatings.Should().Be(expectedAvatage);

        // Assert DB
        using (var assertScope = _factory.Services.CreateScope())
        {
            var db = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var ratings = await db.CourseRatings.Where(x => x.CourseId == courseId).ToListAsync();

            ratings.Should().HaveCount(1);
            var rating = ratings.Single();

            rating.Rating.Should().Be(expectedRating);
            rating.UserId.Should().Be(userId);
            rating.CourseId.Should().Be(courseId);
        }
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenCourseDoesNotExist()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var dto = new CourseRatingDTO { Rating = 5 };

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Post, $"/api/courses/{courseId}/ratings", userId, dto);

        // Act 
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldReturnConflict_WhenRatingAlradyExist()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var dto = new CourseRatingDTO { Rating = 5 };

        using (var arrangeScope = _factory.Services.CreateScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            var course = new Course
            {
                Id = courseId,
                AuthorId = authorId,
                Name = "Test Course",
                Description = "Test Description",
                Price = 0,
                IsFree = true,
                AvatarURL = "http://test.com",
                Status = CourseStatus.Published,
                CourseRatings = new List<CourseRating>
                {
                    new CourseRating
                    {
                        CourseId = courseId,
                        UserId = userId,
                        Rating = 5
                    }
                }
            };

            db.Courses.Add(course);
            await db.SaveChangesAsync();
        }

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Post, $"/api/courses/{courseId}/ratings", userId, dto);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var dto = new CourseRatingDTO
        {
            Rating = 6
        };

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Post, $"/api/courses/{userId}/ratings", courseId, dto);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        content.Should().NotBeNull();
        content.Errors.Should().ContainKey(nameof(CourseRatingDTO.Rating));
    }

    [Fact]
    public async Task ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();
        
        var dto = new CourseRatingDTO
        {
            Rating = 5
        };

        var request = HttpRequestFactory.CreateUnauthorized(HttpMethod.Post, $"/api/courses/{courseId}/ratings", dto);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

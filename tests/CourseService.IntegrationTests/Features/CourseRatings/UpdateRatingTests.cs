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
public class UpdateRatingTests
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public UpdateRatingTests(PostgresContainerFixture postgres)
    {
        _factory = new TestWebApplicationFactory(postgres);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task ShouldUpdateRating_WhenRequestIsValid()
    {
        // Arrange 
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var dto = new CourseRatingDTO { Rating = 5 };

        using (var arrangeScope = _factory.Services.CreateScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var course = new Course
            {
                Id = courseId,
                AuthorId = Guid.NewGuid(),
                Name = "Test",
                Description = "Test",
                Price = 0,
                IsFree = true,
                AvatarURL = "http://test.com",
                Status = CourseStatus.Published
            };

            db.Courses.Add(course);

            var rating = new CourseRating
            {
                CourseId = courseId,
                UserId = userId,
                Rating = 4,
            };

            db.CourseRatings.Add(rating);

            await db.SaveChangesAsync();
        }

        var averageRating = 5.0;
        var amountRating = 1;

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Put, $"/api/courses/{courseId}/ratings", userId, dto);

        // Act
        var response = await _client.SendAsync(request);

        // Assert HTTP
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<CourseRatingResponse>();
        content.Should().NotBeNull();
        content.AverageRating.Should().Be(averageRating);
        content.AmountRatings.Should().Be(amountRating);

        // Assert DB
        using (var assertScope = _factory.Services.CreateScope())
        {
            var db = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var updated = await db.CourseRatings
                .SingleAsync(x => x.CourseId == courseId && x.UserId == userId);
            updated.Rating.Should().Be(dto.Rating);
        }
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenRatingDoesNotExist()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var dto = new CourseRatingDTO { Rating = 5 };

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            db.Courses.Add(new Course
            {
                Id = courseId,
                AuthorId = Guid.NewGuid(),
                Name = "Test",
                Description = "Test",
                Price = 0,
                IsFree = true,
                AvatarURL = "http://test.com",
                Status = CourseStatus.Published
            });

            await db.SaveChangesAsync();
        }

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Put, $"/api/courses/{courseId}/ratings", userId, dto);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be((int)HttpStatusCode.NotFound); 
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var dto = new CourseRatingDTO { Rating = 6 };

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Put, $"/api/courses/{courseId}/ratings", userId, dto);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        content.Should().NotBeNull();
        content!.Errors.Should().ContainKey(nameof(CourseRatingDTO.Rating));
    }

    [Fact]
    public async Task ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var dto = new CourseRatingDTO { Rating = 5 };

        var request = HttpRequestFactory.CreateUnauthorized(HttpMethod.Put, $"/api/courses/{courseId}/ratings", dto);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

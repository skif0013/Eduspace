using CourseService.Application.Courses.DTO;
using CourseService.Domain.Entities;
using CourseService.Domain.Enums;
using CourseService.Infrastructure.Data;
using CourseService.IntegrationTests.Common;
using CourseService.IntegrationTests.Common.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace CourseService.IntegrationTests.Features.Courses;

public class GetCourseByIdTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public GetCourseByIdTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
    }

    [Fact]
    public async Task WhenCourseExists_ShouldReturnCourse()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        using (var arrangeScope = _factory.Services.CreateScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            db.Courses.Add(new Course
            {
                Id = courseId,
                AuthorId = Guid.NewGuid(),
                Name = "Test Course",
                Description = "Test Description",
                Price = 0,
                IsFree = true,
                Status = CourseStatus.Published,
                CourseRatings = new List<CourseRating>
                {
                    new CourseRating
                    {
                        CourseId = courseId,
                        Rating = 5
                    }
                }
            });

            await db.SaveChangesAsync();
        }

        // Act 
        var response = await _client.GetAsync($"/api/courses/{courseId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content
            .ReadFromJsonAsync<CourseResponse>(TestJsonOptions.Default);

        result.Should().NotBeNull();
        result.Id.Should().Be(courseId);
        result.AmountRatings.Should().Be(1);
        result.AverageRating.Should().Be(5);
    }

    [Fact]
    public async Task WhenCourseDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var anotherCourseId = Guid.NewGuid();

        using (var arrangeScope = _factory.Services.CreateScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            db.Courses.Add(new Course
            {
                Id = anotherCourseId,
                AuthorId = Guid.NewGuid(),
                Name = "Test Course",
                Description = "Test Description",
                Price = 0,
                IsFree = true,
                Status = CourseStatus.Published,
            });

            await db.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync($"/api/courses/{courseId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        problem.Should().NotBeNull();
        problem!.Status.Should().Be(404);
    }

    [Fact]
    public async Task WhenCourseIsPaid_ShouldReturnForbidden()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        using (var arrangeScope = _factory.Services.CreateScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            db.Courses.Add(new Course
            {
                Id = courseId,
                AuthorId = Guid.NewGuid(),
                Name = "Test Course",
                Description = "Test Description",
                Price = 100,
                IsFree = false,
                Status = CourseStatus.Published,
            });

            await db.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync($"/api/courses/{courseId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var problem = await response.Content
            .ReadFromJsonAsync<CourseResponse>(TestJsonOptions.Default);

        problem.Should().NotBeNull();
    }
}

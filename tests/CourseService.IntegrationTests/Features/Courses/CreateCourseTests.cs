using CourseService.Application.Courses.DTO;
using CourseService.IntegrationTests.Common;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using CourseService.IntegrationTests.Common.Fixtures;
using System.Text.Json.Serialization;
using System.Text.Json;
using CourseService.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CourseService.IntegrationTests.Common.Helpers;

namespace CourseService.IntegrationTests.Features.Courses;

public class CreateCourseTests : IClassFixture<PostgresContainerFixture>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public CreateCourseTests(PostgresContainerFixture postgres)
    {
        _factory = new TestWebApplicationFactory(postgres);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task ShouldCreateCourse_WhenRequestIsValid()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var authorId = Guid.NewGuid();

        _client.DefaultRequestHeaders.Remove("X-Test-UserId");
        _client.DefaultRequestHeaders.Add("X-Test-UserId", authorId.ToString());

        var dto = new CourseDTO
        {
            Name = "Test Course",
            Description = "Test Description",
            Price = 0,
            IsFree = true,
            AvatarURL = "http://test.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/courses", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var result = await response.Content
            .ReadFromJsonAsync<CourseResponse>(TestJsonOptions.Default);
        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);

        using (var assertScope = _factory.Services.CreateScope())
        {
            var db = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        
            var created = await db.Courses.FirstOrDefaultAsync(c => c.Id == result.Id);

            created.Should().NotBeNull();
            created.Name.Should().Be(dto.Name);
        }
    }

    [Fact]
    public async Task ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var dto = new CourseDTO 
        { 
            Name = "Test Course",
            Description = "Test Description",
            Price = 0,
            IsFree = true,
            AvatarURL = "http://test.com"
        };

        _client.DefaultRequestHeaders.Remove("X-Test-UserId");
        _client.DefaultRequestHeaders.Add("X-Test-Auth-Fail", "true");

        // Act
        var response = await _client.PostAsJsonAsync("/api/courses", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var authorId = Guid.NewGuid();

        _client.DefaultRequestHeaders.Remove("X-Test-UserId");
        _client.DefaultRequestHeaders.Add("X-Test-UserId", authorId.ToString());

        var dto = new CourseDTO
        {
            Name = "",
            Description = "",
            Price = -10,
            IsFree = true,
            AvatarURL = "http://test.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/courses", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

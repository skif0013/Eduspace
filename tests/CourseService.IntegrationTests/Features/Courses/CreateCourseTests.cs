//using CourseService.Application.Courses.DTO;
//using CourseService.Infrastructure.Data;
//using CourseService.IntegrationTests.Common;
//using Microsoft.Extensions.DependencyInjection;
//using System.Net;
//using System.Text.Json.Serialization;
//using System.Text.Json;
//using System.Net.Http.Json;
//using FluentAssertions;
//using Microsoft.EntityFrameworkCore;

//namespace CourseService.IntegrationTests.Features.Courses;

//public class CreateCourseTests : IClassFixture<TestWebApplicationFactory>
//{
//    private readonly HttpClient _client;
//    private readonly TestWebApplicationFactory _factory;

//    public CreateCourseTests(TestWebApplicationFactory factory)
//    {
//        _client = factory.CreateClient();
//        _factory = factory;
//    }

//    [Fact]
//    public async Task ShouldCreateCourse_WhenRequestIsValid_()
//    {
//        // Arrange
//        using var scope = _factory.Services.CreateScope();
//        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

//        db.Database.EnsureDeleted();
//        db.Database.EnsureCreated();

//        var dto = new CourseDTO
//        {
//            Name = "Test Course",
//            Description = "Test Description",
//            Price = 0,
//            IsFree = true,
//            AvatarURL = "http://someting.com"
//        };

//        // Act
//        var response = await _client.PostAsJsonAsync("/api/courses", dto);

//        // Assert
//        response.StatusCode.Should().Be(HttpStatusCode.Created);

//        response.Headers.Location.Should().NotBeNull();

//        var content = await response.Content.ReadAsStringAsync();
//        content.Should().NotBeNullOrEmpty();

//        var result = JsonSerializer.Deserialize<CourseResponse>(content, new JsonSerializerOptions
//        {
//            PropertyNameCaseInsensitive = true,
//            Converters = { new JsonStringEnumConverter() }
//        });

//        result.Should().NotBeNull();
//        result.Name.Should().Be(dto.Name);

//        var courseInDb = await db.Courses.FirstOrDefaultAsync(c => c.Id == result.Id);

//        courseInDb.Should().NotBeNull();
//        courseInDb.Name.Should().Be(dto.Name);
//    }

//    [Fact]
//    public async Task ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
//    {
//        // Arrange
//        var dto = new CourseDTO { Name = "Test" };

//        var request = new HttpRequestMessage(HttpMethod.Post, "/api/courses")
//        {
//            Content = JsonContent.Create(dto)
//        };

//        request.Headers.Add("X-Test-Auth-Fail", "true");

//        // Act
//        var response = await _client.SendAsync(request);

//        // Assert
//        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
//    }

//    [Fact]
//    public async Task ShouldReturnBadRequest_WhenRequestIsInvalid()
//    {
//        // Arrange
//        using var scope = _factory.Services.CreateScope();
//        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

//        db.Database.EnsureDeleted();
//        db.Database.EnsureCreated();

//        //var anotherAuthorId = Guid.NewGuid();

//        var dto = new CourseDTO
//        {
//            Name = "Test Course",
//            Description = "Test Description",
//            Price = 10,
//            IsFree = true,
//            AvatarURL = "http://someting.com"
//        };

//        // Act
//        var response = await _client.PostAsJsonAsync("/api/courses", dto);

//        // Assert
//        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//    }
//}

using CourseService.IntegrationTests.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace CourseService.IntegrationTests.Features.Health;

public class HealthCheckTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HealthCheckTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_UnkhownEndpoint_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/some-random-url");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

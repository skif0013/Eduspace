using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CourseService.IntegrationTests.Common;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("RedisEndPoint", "localhost:6379");
        Environment.SetEnvironmentVariable("RedisUser", "test");
        Environment.SetEnvironmentVariable("RedisPassword", "test");
    }
}

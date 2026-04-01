using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CourseService.IntegrationTests.Common;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("RedisEndPoint", "localhost:6379");
        Environment.SetEnvironmentVariable("RedisUser", "test");
        Environment.SetEnvironmentVariable("RedisPassword", "test");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IDistributedCache>();

            services.AddDistributedMemoryCache();
        });
    }
}

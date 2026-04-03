using CourseService.Application.Caching;
using CourseService.Application.Messaging;
using CourseService.Infrastructure.Data;
using CourseService.IntegrationTests.Common.Fakes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;

namespace CourseService.IntegrationTests.Common;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        Environment.SetEnvironmentVariable("RedisEndPoint", "localhost:6379");
        Environment.SetEnvironmentVariable("RedisUser", "test");
        Environment.SetEnvironmentVariable("RedisPassword", "test");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IDistributedCache>();
            services.RemoveAll<IConnectionMultiplexer>();
            services.RemoveAll<IMessagePublisher>();
            services.RemoveAll<ICourseCache>();
            services.RemoveAll<IRedisKeyBuilder>();

            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<ApplicationDbContext>();

            services.AddDistributedMemoryCache();
            services.AddSingleton<IMessagePublisher, FakeMessagePublisher>();
            services.AddSingleton<ICourseCache, FakeCourseCache>();
            services.AddSingleton<IRedisKeyBuilder, FakeRedisKeyBuilder>();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });
        });
    }
}

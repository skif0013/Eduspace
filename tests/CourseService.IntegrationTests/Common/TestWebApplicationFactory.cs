using CourseService.Application.Caching;
using CourseService.Application.Courses.Validators;
using CourseService.Application.Messaging;
using CourseService.Infrastructure.Data;
using CourseService.IntegrationTests.Common.Fakes;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore.Storage;

namespace CourseService.IntegrationTests.Common;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    //private static readonly InMemoryDatabaseRoot _dbRoot = new();

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
                //options.UseInMemoryDatabase("TestDb", _dbRoot);
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = FakeAuthHandler.Scheme;
                options.DefaultChallengeScheme = FakeAuthHandler.Scheme;
            })
            .AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>(
            FakeAuthHandler.Scheme, _ => { });

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<CourseDtoValidator>();
        });

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
        });
    }
}

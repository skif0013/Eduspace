using CourseService.Application.Caching;
using CourseService.Application.Courses.Interfaces;
using CourseService.Application.Messaging;
using CourseService.Infrastructure.Caching;
using CourseService.Infrastructure.Data;
using CourseService.Infrastructure.Messaging.Redis;
using CourseService.Infrastructure.Repositories;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace CourseService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        LoadEnvironment();

        services.AddDatabase(configuration);

        services.AddRedis(configuration);

        services.AddRepositories();

        return services;
    }

    private static void LoadEnvironment()
    {
        var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
        Env.Load(envPath);
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }

    private static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var redisEndPoint = Environment.GetEnvironmentVariable("RedisEndPoint");
        var redisUser = Environment.GetEnvironmentVariable("RedisUser");
        var redisPassword = Environment.GetEnvironmentVariable("RedisPassword");

        var redisOptions = new ConfigurationOptions
        {
            EndPoints = { redisEndPoint },
            User = redisUser,
            Password = redisPassword,
            AbortOnConnectFail = false,
            ConnectRetry = 5,
            ConnectTimeout = 15000,
            SyncTimeout = 10000,
            KeepAlive = 30,
            ReconnectRetryPolicy = new ExponentialRetry(5000)
        };

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            return ConnectionMultiplexer.Connect(redisOptions);
        });

        services.AddSingleton<IDistributedCache>(sp =>
        {
            var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();

            return new RedisCache(new RedisCacheOptions
            {
                ConnectionMultiplexerFactory = () =>
                    Task.FromResult(multiplexer)
            });
        });

        services.AddSingleton<IMessagePublisher, RedisMessagePublisher>();
        services.AddSingleton<ICourseCache, RedisCourseCache>();

        services.Configure<RadisCacheSettings>(configuration.GetSection("RedisCacheSettings"));

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ICourseRatingRepository, CourseRatingRepository>();

        return services;
    }
}

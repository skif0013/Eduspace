using CourseService.Application.Caching;
using CourseService.Application.Messaging;
using CourseService.Infrastructure.Caching;
using CourseService.Infrastructure.Messaging.Redis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace CourseService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        #region Redis ( Cache and Pub/Sub)
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

        services.AddSingleton<IConnectionMultiplexer>( sp =>
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
        #endregion
        return services;
    }
}

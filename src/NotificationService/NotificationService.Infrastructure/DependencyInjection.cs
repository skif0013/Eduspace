using BuildingBlocks.Redis.Contracts.Broker;
using BuildingBlocks.Redis.Contracts.Serealizer;
using BuildingBlocks.Redis.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationService.Application.Contracts;
using NotificationService.Domain.Models;
using NotificationService.Infrastructure.Redis.RedisBroker;
using NotificationService.Infrastructure.Service;
using NotificationService.Infrastructure.SmtpClientFactory;
using StackExchange.Redis;

namespace NotificationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddEmailServices(configuration);
        services.AddRedis(configuration);

        return services;
    }

    private static IServiceCollection AddEmailServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var emailSettings = new EmailSettings
        {
            SmtpHost = configuration["SmtpSettings:Host"] ?? "smtp.gmail.com",
            SmtpPort = configuration.GetValue("SmtpSettings:Port", 587),
            EnableSsl = configuration.GetValue("SmtpSettings:EnableSsl", true),
            Username = configuration["SmtpSettings:Username"] ?? "default@gmail.com",
            Password = configuration["SmtpSettings:Password"] ?? "default-password",
            FromAddress = configuration["SmtpSettings:SenderEmail"] ?? "no-reply@domain.com"
        };

        services.AddSingleton(emailSettings);
        services.AddSingleton(configuration.GetSection("EmailTemplates:Verification").Get<EmailTemplates>()
                              ?? new EmailTemplates());
        services.AddTransient<IEmailCreateClient, ClientFactory>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }

    private static IServiceCollection AddRedis(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var redisEndPoint = configuration["RedisEndPoint"] ?? "redis:6379";

        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var redisConfig = new ConfigurationOptions
            {
                EndPoints = { redisEndPoint },
                AbortOnConnectFail = false
            };

            return ConnectionMultiplexer.Connect(redisConfig);
        });

        services.AddSingleton<IStreamEventSerializer, StreamEventSerializer>();
        services.AddSingleton<IRedisMessageBroker, RedisMessageBroker>();
        services.AddHostedService<BuildingBlocks.Redis.Events.RedisSubscriberWorker>();

        return services;
    }
}

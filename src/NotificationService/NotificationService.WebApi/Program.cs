using DotNetEnv;
using Microsoft.OpenApi.Models;
using NotificationService.Application.Contracts;
using NotificationService.Application.Interfaces.Services;
using NotificationService.Domain.Models;
using NotificationService.Infrastructure.Redis;
using NotificationService.Infrastructure.Redis.Configuration;
using NotificationService.Infrastructure.Service;
using NotificationService.Infrastructure.Services;
using NotificationService.Infrastructure.SmtpClientFactory;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5005);
    options.ListenAnyIP(5006);
});

var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
Env.Load(envPath);

RegisterEmailServices(builder.Services);
RegisterRedisServices(builder.Services);
RegisterNotificationHandlers(builder.Services);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
ConfigureSwagger(builder.Services);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NotificationService API v1"));
}

app.MapControllers();
app.UseHttpsRedirection();
app.Run();

static void RegisterEmailServices(IServiceCollection services)
{
    var emailSettings = new EmailSettings
    {
        SmtpHost = Environment.GetEnvironmentVariable("SmtpSettings__Host") ?? "smtp.gmail.com",
        SmtpPort = int.TryParse(Environment.GetEnvironmentVariable("SmtpSettings__Port"), out var port) ? port : 587,
        EnableSsl = bool.TryParse(Environment.GetEnvironmentVariable("SmtpSettings__EnableSsl"), out var ssl) && ssl,
        Username = Environment.GetEnvironmentVariable("SmtpSettings__Username") ?? "default@gmail.com",
        Password = Environment.GetEnvironmentVariable("SmtpSettings__Password") ?? "default-password",
        FromAddress = Environment.GetEnvironmentVariable("SmtpSettings__SenderEmail") ?? "no-reply@domain.com"
    };

    services.AddSingleton(emailSettings);
    services.AddScoped<IEmailService, EmailService>();
    services.AddTransient<IEmailCreateClient, ClientFactory>();
    services.AddScoped<IMessageService, MessageService>();
}

static void RegisterRedisServices(IServiceCollection services)
{
    var redisEndPoint = Environment.GetEnvironmentVariable("RedisEndPoint") ?? "localhost:6379";
    var quizFinishedStream = Environment.GetEnvironmentVariable("Redis__Streams__QuizFinished") ?? "quiz:finished:v1";
    var consumerGroupName = "notification-service";
    var consumerName = "notification-worker-1";

    var redisStreamConfig = new RedisStreamConsumerConfiguration(
        quizFinishedStream,
        consumerGroupName,
        consumerName);

    services.AddSingleton(redisStreamConfig);

    services.AddSingleton<IConnectionMultiplexer>(sp =>
    {
        var config = new ConfigurationOptions
        {
            EndPoints = { redisEndPoint },
            AbortOnConnectFail = false
        };
        return ConnectionMultiplexer.Connect(config);
    });

    services.AddHostedService<RedisStreamSubscriberService>();
}

static void RegisterNotificationHandlers(IServiceCollection services)
{
    services.AddSingleton<IMessageHandler, ConfimEmailHandler>();
    services.AddSingleton<IMessageHandler, QuizFinishedEmailHandler>();
}

static void ConfigureSwagger(IServiceCollection services)
{
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "NotificationService API",
            Version = "v1"
        });
    });
}


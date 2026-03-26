using DotNetEnv;
using Microsoft.OpenApi.Models;
using NotificationService.Application.Contracts;
using NotificationService.Application.Interfaces.Services;
using NotificationService.Domain.Models;
using NotificationService.Infrastructure.Redis;
using NotificationService.Infrastructure.Service;
using NotificationService.Infrastructure.Services;
using NotificationService.Infrastructure.SmtpClientFactory;

var builder = WebApplication.CreateBuilder(args);

var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
Env.Load(envPath);


var emailSettings = new EmailSettings
{
    SmtpHost = Environment.GetEnvironmentVariable("SmtpSettings__Host") ?? "smtp.gmail.com",
    SmtpPort = int.TryParse(Environment.GetEnvironmentVariable("SmtpSettings__Port"), out var port) ? port : 587,
    EnableSsl = bool.TryParse(Environment.GetEnvironmentVariable("SmtpSettings__EnableSsl"), out var ssl) && ssl,
    Username = Environment.GetEnvironmentVariable("SmtpSettings__Username") ?? "default@gmail.com",
    Password = Environment.GetEnvironmentVariable("SmtpSettings__Password") ?? "default-password",
    FromAddress = Environment.GetEnvironmentVariable("SmtpSettings__SenderEmail") ?? "no-reply@domain.com"
};


builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddSingleton<IMessageHandler, ConfimEmailHandler>();
builder.Services.AddHostedService<RedisSubscriberService>();

var redisEndPoint = builder.Configuration.GetValue<string>("RedisEndPoint");
var redisUser = builder.Configuration.GetValue<string>("RedisUser");
var redisPassword = builder.Configuration.GetValue<string>("RedisPassword");

Console.WriteLine($"Redis EndPoint: {redisEndPoint}");
Console.WriteLine($"Redis User: {redisUser}");
Console.WriteLine($"Redis Password: {redisPassword}");

builder.Services.AddSingleton<RedisMessageBroker>(sb =>
{
    var config = new StackExchange.Redis.ConfigurationOptions
    {
        EndPoints = { redisEndPoint },
        User = redisUser,
        Password = redisPassword,
        AbortOnConnectFail = false
    };
    var connectionString = config.ToString();
    return new RedisMessageBroker(connectionString);
});




builder.Services.AddSingleton(emailSettings);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "NotificationService API", Version = "v1" });
});


builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddTransient<IEmailCreateClient, ClientFactory>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NotificationService API v1"));
}

app.MapControllers();
app.UseHttpsRedirection();
app.Run();

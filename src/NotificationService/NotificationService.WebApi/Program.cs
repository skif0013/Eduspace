using Microsoft.OpenApi.Models;
using NotificationService.Application.Contracts;
using NotificationService.Application.Interfaces.Services;
using NotificationService.Domain.Models;
using NotificationService.Infrastructure.Redis;
using NotificationService.Infrastructure.Service;
using NotificationService.Infrastructure.Services;
using NotificationService.Infrastructure.SmtpClientFactory;

var builder = WebApplication.CreateBuilder(args);

var emailSettings = new EmailSettings
{
    SmtpHost = builder.Configuration.GetValue<string>("SmtpSettings:Host"),
    SmtpPort = builder.Configuration.GetValue<int>("SmtpSettings:Port"),
    EnableSsl = builder.Configuration.GetValue<bool>("SmtpSettings:EnableSsl"),
    Username = builder.Configuration.GetValue<string>("SmtpSettings:Username"),
    Password = builder.Configuration.GetValue<string>("SmtpSettings:Password"),
    FromAddress = builder.Configuration.GetValue<string>("SmtpSettings:SenderEmail")
};

builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddSingleton<IMessageHandler, ConfimEmailHandler>();
builder.Services.AddHostedService<RedisSubscriberService>();

var redisEndPoint = builder.Configuration.GetValue<string>("RedisEndPoint");
var redisUser = builder.Configuration.GetValue<string>("RedisUser");
var redisPassword = builder.Configuration.GetValue<string>("RedisPassword");

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
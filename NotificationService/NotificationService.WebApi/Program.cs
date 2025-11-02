using DotNetEnv;
using Microsoft.OpenApi.Models;
using NotificationService.Application.Contracts;
using NotificationService.Domainn.Models;
using NotificationService.Infrastructure.Service;
using NotificationService.Infrastructure.SmtpClientFactory;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
Env.Load();


var emailSettings = new EmailSettings
{
    SmtpHost = Environment.GetEnvironmentVariable("SmtpSettings__Host") ?? "smtp.gmail.com",
    SmtpPort = int.TryParse(Environment.GetEnvironmentVariable("SmtpSettings__Port"), out var port) ? port : 587,
    EnableSsl = bool.TryParse(Environment.GetEnvironmentVariable("SmtpSettings__EnableSsl"), out var ssl) && ssl,
    Username = Environment.GetEnvironmentVariable("SmtpSettings__Username") ?? "default@gmail.com",
    Password = Environment.GetEnvironmentVariable("SmtpSettings__Password") ?? "default-password",
    FromAddress = Environment.GetEnvironmentVariable("SmtpSettings__SenderEmail") ?? "no-reply@domain.com"
};

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

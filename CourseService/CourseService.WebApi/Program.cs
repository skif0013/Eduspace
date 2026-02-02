using CourseService.Application;
using CourseService.Application.Interfaces.Repositories;
using CourseService.Application.Interfaces.Services;
using CourseService.Application.Messaging;
using CourseService.Application.Services;
using CourseService.Infrastructure.Data;
using CourseService.Infrastructure.Messaging.Redis;
using CourseService.Infrastructure.Repositories;
using CourseService.WebApi;
using CourseService.WebApi.Middlewares;
using DotNetEnv;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

#region Dependency Injection Configuration
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddWebApiServices(builder.Configuration);
#endregion

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ICourseRatingRepository, CourseRatingRepository>();

builder.Services.AddScoped<ICourseService, CourseService.Application.Services.CourseService>();
builder.Services.AddScoped<ICourseRatingService, CourseRatingService>();

var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
Env.Load(envPath);

var redisEndPoint = Environment.GetEnvironmentVariable("RedisEndPoint");
var redisUser = Environment.GetEnvironmentVariable("RedisUser");
var redisPassword = Environment.GetEnvironmentVariable("RedisPassword");

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
builder.Services.AddSingleton<IMessagePublisher, RedisMessagePublisher>();

var app = builder.Build();

app.UseMiddleware<CustomExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

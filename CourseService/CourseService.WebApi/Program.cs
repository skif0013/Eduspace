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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
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

#region config jwt
var validIssuer = builder.Configuration.GetValue<string>("JwtTokenSettings:ValidIssuer");
var validAudience = builder.Configuration.GetValue<string>("JwtTokenSettings:ValidAudience");
var symmetricSecurityKey = builder.Configuration.GetValue<string>("JwtTokenSettings:SymmetricSecurityKey");

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.IncludeErrorDetails = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ClockSkew = TimeSpan.Zero,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = validIssuer,
            ValidAudience = validAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(symmetricSecurityKey)
            ),
        };
    });
#endregion

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

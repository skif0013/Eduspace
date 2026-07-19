
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuizService.Application;
using QuizService.Infrastructure;
using QuizService.Infrastructure.Data;
using System.Text;
using BuildingBlock.UserContextMiddleware.Middleware;
using BuildingBlock.UserContextMiddleware.Models;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using QuizService.Infrastructure.Data;
using QuizService.Application.Contracts;
using QuizService.Application.Contracts.IQuizAttempt;
using QuizService.Application.Contracts.QuestionsContract;
using QuizService.Infrastructure.Redis;
using QuizService.Infrastructure.Redis.Configuration;
using QuizService.Infrastructure.Repositories;
using QuizService.Infrastructure.Persistence.UnitOfWork;
using QuizService.Application.Repositories;
using QuizService.Application.Services;
using BuildingBlocks.Redis.Contracts;
using BuildingBlocks.Redis.Contracts.Serealizer;
using BuildingBlocks.Redis.Serialization;
using Microsoft.OpenApi.Models;
//using Microsoft.OpenApi;
using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);

var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (File.Exists(envPath))
{
    DotNetEnv.Env.Load(envPath);
}

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddQuizApplicationServices();
builder.Services.AddQuizInfrastructure(builder.Configuration, builder.Environment);
builder.Services.AddScoped<UserContext>();
ConfigureAuthentication(builder);


builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Quiz Service API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//ConfigureSwagger(builder.Services);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    if (!app.Environment.IsEnvironment("Testing"))
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseSwagger();
app.UseSwaggerUI(options =>
{ 
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Quiz Service API v1");
    options.RoutePrefix = "swagger"; 
});



app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<UserContextMiddleware>();
app.MapControllers();
app.Run();

static void RegisterApplicationServices(IServiceCollection services)
{
    services.AddScoped<IQuizService, QuizService.Application.Services.QuizService>();
    services.AddScoped<IQuizRepository, QuizRepository>();
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    services.AddScoped<IQuizMapper, QuizMapper>();
    services.AddScoped<IQuestionService, QuestionService>();
    services.AddScoped<IQuestionScoringService, QuestionScoringService>();
    services.AddScoped<IQuestionRepository, QuestionRepository>();
    services.AddScoped<IQuestionMapper, QuestionMapper>();
    services.AddScoped<IAttemptRepository, AttemptRepository>();
    services.AddScoped<IAttemptService, AttemptService>();
    services.AddScoped<UserContext>();
}

static void RegisterRedisServices(IServiceCollection services, IConfiguration configuration)
{
    var redisEndpoint = configuration["Redis:Endpoint"] ?? "localhost:6379";
    var redisUser = configuration["Redis:User"];
    var redisPassword = configuration["Redis:Password"];
    var quizFinishedStream = configuration["Redis:Streams:QuizFinished"] ?? "quiz:finished:v1";

    var redisConfig = new RedisStreamPublisherConfiguration(
        quizFinishedStream,
        redisEndpoint,
        redisUser,
        redisPassword);

    services.AddSingleton(redisConfig);

    services.AddSingleton<ConnectionMultiplexer>(_ =>
    {
        var config = redisConfig.BuildConfigurationOptions();
        return ConnectionMultiplexer.Connect(config);
    });
}

static void ConfigureAuthentication(WebApplicationBuilder builder)
{
    var configuration = builder.Configuration;

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JwtTokenSettings:ValidIssuer"] ?? "your_issuer",
                ValidAudience = configuration["JwtTokenSettings:ValidAudience"] ?? "your_audience",
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["JwtTokenSettings:SymmetricSecurityKey"] ?? "super_secret_key"))
            };
        });
}

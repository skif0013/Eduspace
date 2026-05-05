using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuizService.Infrastructure.Data;
using QuizService.Application.Contracts;
using QuizService.Application.Contracts.IQuizAttempt;
using QuizService.Application.Contracts.QuestionsContract;
using QuizService.Infrastructure.Redis;
using QuizService.Infrastructure.Redis.Configuration;
using QuizService.Infrastructure.Redis.Serialization;
using QuizService.Infrastructure.Repositories;
using QuizService.Infrastructure.Persistence.UnitOfWork;
using QuizService.Application.Repositories;
using QuizService.Application.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var connectionString = configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

RegisterApplicationServices(builder.Services);
RegisterRedisServices(builder.Services, configuration);
ConfigureAuthentication(builder);

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
ConfigureSwagger(builder.Services);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Quiz Service API v1");
    c.RoutePrefix = "";
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

static void RegisterApplicationServices(IServiceCollection services)
{
    services.AddScoped<IQuizService, QuizService.Application.Services.QuizService>();
    services.AddScoped<IQuizRepository, QuizRepository>();
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    services.AddScoped<IQuizMapper, QuizMapper>();
    services.AddScoped<ITokenService, TokenService>();
    services.AddScoped<IQuestionService, QuestionService>();
    services.AddScoped<IQuestionScoringService, QuestionScoringService>();
    services.AddScoped<IQuestionRepository, QuestionRepository>();
    services.AddScoped<IQuestionMapper, QuestionMapper>();
    services.AddScoped<IAttemptRepository, AttemptRepository>();
    services.AddScoped<IAttemptService, AttemptService>();
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

    services.AddSingleton<IStreamEventSerializer, JsonStreamEventSerializer>();

    services.AddScoped<IQuizFinishedEventPublisher>(sp =>
        new QuizFinishedEventStreamPublisher(
            sp.GetRequiredService<ConnectionMultiplexer>(),
            redisConfig,
            sp.GetRequiredService<IStreamEventSerializer>()));
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
                    Encoding.UTF8.GetBytes(configuration["JwtTokenSettings:Secret"] ?? "super_secret_key"))
            };
        });
}

static void ConfigureSwagger(IServiceCollection services)
{
    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Quiz Service API",
            Version = "v1"
        });

        var securityScheme = new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer {token}'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        };

        options.AddSecurityDefinition("Bearer", securityScheme);
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { securityScheme, Array.Empty<string>() }
        });
    });
}

